var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                return uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                    || uri.Host.Equals("127.0.0.1");
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

app.UseCors("LocalDev");
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/calculate", (CalculationRequest request) =>
{
    if (request.Lambda <= 0 || request.Mu <= 0)
    {
        return Results.BadRequest(new { error = "Arrival and service rates must be greater than 0." });
    }

    var lambda = QueueCalculator.ToPerHourRate(request.Lambda, request.Unit);
    var mu = QueueCalculator.ToPerHourRate(request.Mu, request.Unit);

    if (lambda <= 0 || mu <= 0)
    {
        return Results.BadRequest(new { error = "Converted rates must be greater than 0." });
    }

    CalculationResponse response;
    switch (request.Model?.Trim().ToLowerInvariant())
    {
        case "mm1":
            if (lambda / mu >= 1)
            {
                return Results.BadRequest(new { error = "System is unstable because utilization (rho) is greater than or equal to 1." });
            }
            response = QueueCalculator.CalculateMm1(lambda, mu, request.Unit);
            break;
        case "mg1":
            if (!request.Variance.HasValue || request.Variance.Value < 0)
            {
                return Results.BadRequest(new { error = "M/G/1 requires a non-negative service time variance." });
            }
            if (lambda / mu >= 1)
            {
                return Results.BadRequest(new { error = "System is unstable because utilization (rho) is greater than or equal to 1." });
            }
            response = QueueCalculator.CalculateMg1(lambda, mu, request.Variance.Value, request.Unit);
            break;
        case "gg1":
            if (!request.Ca.HasValue || !request.Cs.HasValue || request.Ca.Value < 0 || request.Cs.Value < 0)
            {
                return Results.BadRequest(new { error = "G/G/1 requires non-negative Ca and Cs values." });
            }
            if (lambda / mu >= 1)
            {
                return Results.BadRequest(new { error = "System is unstable because utilization (rho) is greater than or equal to 1." });
            }
            response = QueueCalculator.CalculateGg1(lambda, mu, request.Ca.Value, request.Cs.Value, request.Unit);
            break;
        case "mms":
            if (!request.Servers.HasValue || request.Servers.Value <= 0)
            {
                return Results.BadRequest(new { error = "M/M/s requires a server count greater than 0." });
            }
            if (lambda / (request.Servers.Value * mu) >= 1)
            {
                return Results.BadRequest(new { error = "System is unstable because utilization (rho) is greater than or equal to 1." });
            }
            response = QueueCalculator.CalculateMms(lambda, mu, request.Servers.Value, request.Unit);
            break;
        case "mgc":
            if (!request.Servers.HasValue || request.Servers.Value <= 0)
            {
                return Results.BadRequest(new { error = "M/G/c requires a server count greater than 0." });
            }
            if (!request.Cs2.HasValue || request.Cs2.Value < 0)
            {
                return Results.BadRequest(new { error = "M/G/c requires a non-negative Cs^2 value." });
            }
            if (lambda / (request.Servers.Value * mu) >= 1)
            {
                return Results.BadRequest(new { error = "System is unstable because utilization (rho) is greater than or equal to 1." });
            }
            response = QueueCalculator.CalculateMgc(lambda, mu, request.Servers.Value, request.Cs2.Value, request.Unit);
            break;
        default:
            return Results.BadRequest(new { error = "Unknown model selected." });
    }

    return Results.Ok(response);
});

app.Run();

public record CalculationRequest(
    string Model,
    double Lambda,
    double Mu,
    string Unit,
    double? Variance,
    double? Ca,
    double? Cs,
    int? Servers,
    double? Cs2
);

public record CalculationResponse(
    string Model,
    double Rho,
    double L,
    double Lq,
    double W,
    double Wq,
    string Unit
);

public static class QueueCalculator
{
    private static double Factorial(int n)
    {
        var fact = 1.0;
        for (var i = 2; i <= n; i++)
        {
            fact *= i;
        }

        return fact;
    }

    public static double ToPerHourRate(double value, string? unit)
    {
        var normalized = unit?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "sec" or "s" => value * 3600.0,
            "min" or "m" => value * 60.0,
            "hr" or "h" => value,
            _ => value
        };
    }

    private static double FromHours(double value, string? unit)
    {
        var normalized = unit?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "sec" or "s" => value * 3600.0,
            "min" or "m" => value * 60.0,
            "hr" or "h" => value,
            _ => value
        };
    }

    public static CalculationResponse CalculateMm1(double lambda, double mu, string unit)
    {
        var rho = lambda / mu;
        var l = lambda / (mu - lambda);
        var lq = (lambda * lambda) / (mu * (mu - lambda));
        var w = 1 / (mu - lambda);
        var wq = lambda / (mu * (mu - lambda));

        return new CalculationResponse(
            "M/M/1",
            Math.Round(rho, 3),
            Math.Round(l, 3),
            Math.Round(lq, 3),
            Math.Round(FromHours(w, unit), 3),
            Math.Round(FromHours(wq, unit), 3),
            unit
        );
    }

    public static CalculationResponse CalculateMg1(double lambda, double mu, double variance, string unit)
    {
        var rho = lambda / mu;
        var wq = (lambda * variance + (rho * rho)) / (2 * (1 - rho));
        var lq = lambda * wq;
        var w = wq + (1 / mu);
        var l = lambda * w;

        return new CalculationResponse(
            "M/G/1",
            Math.Round(rho, 3),
            Math.Round(l, 3),
            Math.Round(lq, 3),
            Math.Round(FromHours(w, unit), 3),
            Math.Round(FromHours(wq, unit), 3),
            unit
        );
    }

    public static CalculationResponse CalculateGg1(double lambda, double mu, double ca, double cs, string unit)
    {
        var rho = lambda / mu;
        var wq = (rho / (1 - rho)) * ((ca * ca + cs * cs) / 2) * (1 / mu);
        var lq = lambda * wq;
        var w = wq + (1 / mu);
        var l = lambda * w;

        return new CalculationResponse(
            "G/G/1",
            Math.Round(rho, 3),
            Math.Round(l, 3),
            Math.Round(lq, 3),
            Math.Round(FromHours(w, unit), 3),
            Math.Round(FromHours(wq, unit), 3),
            unit
        );
    }

    public static CalculationResponse CalculateMms(double lambda, double mu, int servers, string unit)
    {
        var rho = lambda / (servers * mu);
        var alpha = lambda / mu;
        var sum = 0.0;
        for (var n = 0; n < servers; n++)
        {
            sum += Math.Pow(alpha, n) / Factorial(n);
        }

        var tail = Math.Pow(alpha, servers) / (Factorial(servers) * (1 - rho));
        var p0 = 1 / (sum + tail);

        var lq = (p0 * Math.Pow(alpha, servers) * rho) /
                 (Factorial(servers) * Math.Pow(1 - rho, 2));

        var wq = lq / lambda;
        var w = wq + (1 / mu);
        var l = lambda * w;

        return new CalculationResponse(
            "M/M/s",
            Math.Round(rho, 3),
            Math.Round(l, 3),
            Math.Round(lq, 3),
            Math.Round(FromHours(w, unit), 3),
            Math.Round(FromHours(wq, unit), 3),
            unit
        );
    }

    public static CalculationResponse CalculateMgc(double lambda, double mu, int servers, double cs2, string unit)
    {
        var rho = lambda / (servers * mu);
        var exponent = Math.Sqrt(2 * (servers + 1)) - 1;
        var wq = ((cs2 + 1) / 2.0) *
                 (Math.Pow(rho, exponent) / (servers * (1 - rho))) *
                 (1 / mu);

        var lq = lambda * wq;
        var w = wq + (1 / mu);
        var l = lambda * w;

        return new CalculationResponse(
            "M/G/c",
            Math.Round(rho, 3),
            Math.Round(l, 3),
            Math.Round(lq, 3),
            Math.Round(FromHours(w, unit), 3),
            Math.Round(FromHours(wq, unit), 3),
            unit
        );
    }
}
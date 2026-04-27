var builder = WebApplication.CreateBuilder(args);
var configuredOrigins = builder.Configuration["Cors:AllowedOrigins"];
var allowedOrigins = (configuredOrigins ?? string.Empty)
    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

var defaultDevOrigins = new[]
{
    "http://localhost:5000",
    "https://localhost:5001",
    "http://127.0.0.1:5000",
    "https://127.0.0.1:5001"
};

var effectiveOrigins = allowedOrigins.Length > 0 ? allowedOrigins : defaultDevOrigins;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors", policy =>
    {
        policy
            .WithOrigins(effectiveOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

app.UseCors("AppCors");
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
            response = QueueCalculator.CalculateMg1(lambda, mu, QueueCalculator.ToHoursSquared(request.Variance.Value, request.Unit), request.Unit);
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
        case "ggc":
            if (!request.Servers.HasValue || request.Servers.Value <= 0)
            {
                return Results.BadRequest(new { error = "G/G/c requires a server count greater than 0." });
            }
            if (!request.Ca.HasValue || !request.Cs.HasValue || request.Ca.Value < 0 || request.Cs.Value < 0)
            {
                return Results.BadRequest(new { error = "G/G/c requires non-negative Ca and Cs values." });
            }
            if (lambda / (request.Servers.Value * mu) >= 1)
            {
                return Results.BadRequest(new { error = "System is unstable because utilization (rho) is greater than or equal to 1." });
            }
            response = QueueCalculator.CalculateGgc(lambda, mu, request.Servers.Value, request.Ca.Value, request.Cs.Value, request.Unit);
            break;
        default:
            return Results.BadRequest(new { error = "Unknown model selected." });
    }

    return Results.Ok(response);
});

app.Run();
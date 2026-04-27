using SimulatorFinalProject.Simulator.Services;

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
builder.Services.AddControllers();
builder.Services.AddScoped<SimulationService>();

var app = builder.Build();

app.UseCors("AppCors");
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
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
            if (string.IsNullOrWhiteSpace(request.Mg1Distribution))
            {
                return Results.BadRequest(new { error = "M/G/1 requires a service distribution selection." });
            }
            var mg1Distribution = request.Mg1Distribution.Trim().ToLowerInvariant();
            double meanServiceHours;
            double varianceHoursSquared;

            switch (mg1Distribution)
            {
                case "exponential":
                    if (!request.Mg1MeanServiceTime.HasValue || request.Mg1MeanServiceTime.Value <= 0)
                    {
                        return Results.BadRequest(new { error = "M/G/1 exponential requires a positive mean service time." });
                    }

                    meanServiceHours = QueueUnitConverter.ToHours(request.Mg1MeanServiceTime.Value, request.Unit);
                    varianceHoursSquared = Math.Pow(meanServiceHours, 2);
                    break;
                case "uniform":
                    if (!request.Mg1MinServiceTime.HasValue || !request.Mg1MaxServiceTime.HasValue)
                    {
                        return Results.BadRequest(new { error = "M/G/1 uniform requires min and max service time values." });
                    }

                    if (request.Mg1MinServiceTime.Value < 0 || request.Mg1MaxServiceTime.Value < 0 || request.Mg1MaxServiceTime.Value < request.Mg1MinServiceTime.Value)
                    {
                        return Results.BadRequest(new { error = "M/G/1 uniform requires min/max service time values with max >= min and both non-negative." });
                    }

                    var minServiceHours = QueueUnitConverter.ToHours(request.Mg1MinServiceTime.Value, request.Unit);
                    var maxServiceHours = QueueUnitConverter.ToHours(request.Mg1MaxServiceTime.Value, request.Unit);
                    meanServiceHours = (minServiceHours + maxServiceHours) / 2.0;
                    varianceHoursSquared = Math.Pow(maxServiceHours - minServiceHours, 2) / 12.0;
                    break;
                case "general":
                    if (!request.Mg1MeanServiceTime.HasValue || !request.Mg1Variance.HasValue)
                    {
                        return Results.BadRequest(new { error = "M/G/1 general requires mean and variance values." });
                    }

                    if (request.Mg1MeanServiceTime.Value <= 0 || request.Mg1Variance.Value < 0)
                    {
                        return Results.BadRequest(new { error = "M/G/1 general requires positive mean and non-negative variance." });
                    }

                    meanServiceHours = QueueUnitConverter.ToHours(request.Mg1MeanServiceTime.Value, request.Unit);
                    varianceHoursSquared = QueueUnitConverter.ToHoursSquared(request.Mg1Variance.Value, request.Unit);
                    break;
                default:
                    return Results.BadRequest(new { error = "M/G/1 requires a valid service distribution selection." });
            }

            var muFromMean = 1 / meanServiceHours;
            if (lambda / muFromMean >= 1)
            {
                return Results.BadRequest(new { error = "System is unstable because utilization (rho) is greater than or equal to 1." });
            }

            response = QueueCalculator.CalculateMg1(lambda, meanServiceHours, varianceHoursSquared, request.Unit);
            break;
        case "gg1":
            if (!request.Ca.HasValue || !request.Cs.HasValue || request.Ca.Value < 0 || request.Cs.Value < 0)
            {
                return Results.BadRequest(new { error = "G/G/1 requires non-negative arrival/service variance values." });
            }
            if (lambda / mu >= 1)
            {
                return Results.BadRequest(new { error = "System is unstable because utilization (rho) is greater than or equal to 1." });
            }
            response = QueueCalculator.CalculateGg1(
                lambda,
                mu,
                QueueCalculator.ToHoursSquared(request.Ca.Value, request.Unit),
                QueueCalculator.ToHoursSquared(request.Cs.Value, request.Unit),
                request.Unit
            );
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
                return Results.BadRequest(new { error = "M/G/c requires a non-negative Cs value." });
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

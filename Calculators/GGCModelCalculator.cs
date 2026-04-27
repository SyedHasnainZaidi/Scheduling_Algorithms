public static class GGCModelCalculator
{
    public static CalculationResponse Calculate(double lambda, double mu, int servers, double ca, double cs, string unit)
    {
        var rho = lambda / (servers * mu);
        var alpha = lambda / mu;

        var p0 = QueueMath.CalculateP0Mmc(lambda, mu, servers);

        var lqMm = (p0 * Math.Pow(alpha, servers) * rho) /
                   (QueueMath.Factorial(servers) * Math.Pow(1 - rho, 2));

        var variabilityFactor = ((ca * ca) + (cs * cs)) / 2.0;
        var lq = lqMm * variabilityFactor;
        var wq = lq / lambda;
        var w = wq + (1 / mu);
        var l = lambda * w;

        return new CalculationResponse(
            "G/G/c",
            Math.Round(rho, 3),
            Math.Round(p0, 3),
            Math.Round(l, 3),
            Math.Round(lq, 3),
            Math.Round(QueueUnitConverter.FromHours(w, unit), 3),
            Math.Round(QueueUnitConverter.FromHours(wq, unit), 3),
            null,
            null,
            unit
        );
    }
}

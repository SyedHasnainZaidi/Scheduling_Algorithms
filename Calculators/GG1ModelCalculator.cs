public static class GG1ModelCalculator
{
    public static CalculationResponse Calculate(double lambda, double mu, double varArrival, double varService, string unit)
    {
        var rho = lambda / mu;
        var meanInterarrival = 1 / lambda;
        var meanService = 1 / mu;

        var ca2 = varArrival / Math.Pow(meanInterarrival, 2);
        var cs2 = varService / Math.Pow(meanService, 2);

        var rho2 = Math.Pow(rho, 2);
        var numerator = rho2 * (1 + cs2) * (ca2 + rho2 * cs2);
        var denominator = 2 * (1 - rho) * (1 + rho2 * cs2);
        var lq = numerator / denominator;

        var wq = lq / lambda;
        var w = wq + meanService;
        var l = lambda * w;
        var p0 = 1 - rho;

        return new CalculationResponse(
            "G/G/1",
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

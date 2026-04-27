public static class MG1ModelCalculator
{
    public static CalculationResponse Calculate(double lambda, double meanServiceHours, double varianceHoursSquared, string unit)
    {
        var mu = 1 / meanServiceHours;
        var rho = lambda / mu;
        var lq = (Math.Pow(lambda, 2) * varianceHoursSquared + Math.Pow(rho, 2)) / (2 * (1 - rho));
        var wq = lq / lambda;
        var w = wq + meanServiceHours;
        var l = lambda * w;
        var p0 = 1 - rho;

        return new CalculationResponse(
            "M/G/1",
            Math.Round(rho, 3),
            Math.Round(p0, 3),
            Math.Round(l, 3),
            Math.Round(lq, 3),
            Math.Round(QueueUnitConverter.FromHours(w, unit), 3),
            Math.Round(QueueUnitConverter.FromHours(wq, unit), 3),
            Math.Round(QueueUnitConverter.FromHours(meanServiceHours, unit), 3),
            Math.Round(QueueUnitConverter.FromHoursSquared(varianceHoursSquared, unit), 3),
            unit
        );
    }
}

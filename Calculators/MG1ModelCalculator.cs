public static class MG1ModelCalculator
{
    public static CalculationResponse Calculate(double lambda, double mu, double varianceHoursSquared, string unit)
    {
        var rho = lambda / mu;
        var p0 = 1 - rho;
        var meanServiceTime = 1 / mu;
        var secondMoment = varianceHoursSquared + (meanServiceTime * meanServiceTime);
        var wq = (lambda * secondMoment) / (2 * (1 - rho));
        var lq = lambda * wq;
        var w = wq + (1 / mu);
        var l = lambda * w;

        return new CalculationResponse(
            "M/G/1",
            Math.Round(rho, 3),
            Math.Round(p0, 3),
            Math.Round(l, 3),
            Math.Round(lq, 3),
            Math.Round(QueueUnitConverter.FromHours(w, unit), 3),
            Math.Round(QueueUnitConverter.FromHours(wq, unit), 3),
            unit
        );
    }
}

public static class MM1ModelCalculator
{
    public static CalculationResponse Calculate(double lambda, double mu, string unit)
    {
        var rho = lambda / mu;
        var p0 = 1 - rho;
        var l = lambda / (mu - lambda);
        var lq = (lambda * lambda) / (mu * (mu - lambda));
        var w = 1 / (mu - lambda);
        var wq = lambda / (mu * (mu - lambda));

        return new CalculationResponse(
            "M/M/1",
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

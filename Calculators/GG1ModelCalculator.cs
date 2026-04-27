public static class GG1ModelCalculator
{
    public static CalculationResponse Calculate(double lambda, double mu, double ca, double cs, string unit)
    {
        var rho = lambda / mu;
        var p0 = 1 - rho;
        var wq = (rho / (1 - rho)) * ((ca * ca + cs * cs) / 2) * (1 / mu);
        var lq = lambda * wq;
        var w = wq + (1 / mu);
        var l = lambda * w;

        return new CalculationResponse(
            "G/G/1",
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

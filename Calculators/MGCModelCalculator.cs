public static class MGCModelCalculator
{
    public static CalculationResponse Calculate(double lambda, double mu, int servers, double cs2, string unit)
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
            null,
            Math.Round(l, 3),
            Math.Round(lq, 3),
            Math.Round(QueueUnitConverter.FromHours(w, unit), 3),
            Math.Round(QueueUnitConverter.FromHours(wq, unit), 3),
            unit
        );
    }
}

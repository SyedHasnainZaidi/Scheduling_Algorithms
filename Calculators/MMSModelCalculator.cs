public static class MMSModelCalculator
{
    public static CalculationResponse Calculate(double lambda, double mu, int servers, string unit)
    {
        var rho = lambda / (servers * mu);
        var alpha = lambda / mu;
        var sum = 0.0;
        for (var n = 0; n < servers; n++)
        {
            sum += Math.Pow(alpha, n) / QueueMath.Factorial(n);
        }

        var tail = Math.Pow(alpha, servers) / (QueueMath.Factorial(servers) * (1 - rho));
        var p0 = 1 / (sum + tail);

        var lq = (p0 * Math.Pow(alpha, servers) * rho) /
                 (QueueMath.Factorial(servers) * Math.Pow(1 - rho, 2));

        var wq = lq / lambda;
        var w = wq + (1 / mu);
        var l = lambda * w;

        return new CalculationResponse(
            "M/M/s",
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

public static class QueueMath
{
    public static double Factorial(int n)
    {
        var fact = 1.0;
        for (var i = 2; i <= n; i++)
        {
            fact *= i;
        }

        return fact;
    }

    public static double CalculateP0Mmc(double lambda, double mu, int servers)
    {
        var sum = 0.0;
        var alpha = lambda / mu;

        for (var n = 0; n < servers; n++)
        {
            sum += Math.Pow(alpha, n) / Factorial(n);
        }

        var rho = lambda / (servers * mu);
        var tail = Math.Pow(alpha, servers) / (Factorial(servers) * (1 - rho));

        return 1 / (sum + tail);
    }
}

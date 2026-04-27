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
}

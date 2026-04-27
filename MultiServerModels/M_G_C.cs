using System;

class MGCQueue
{
    static double Factorial(int n)
    {
        double result = 1;
        for (int i = 2; i <= n; i++)
            result *= i;
        return result;
    }

    static double CalculateP0(double lambda, double mu, int c)
    {
        double sum = 0.0;
        double a = lambda / mu;
        double rho = lambda / (c * mu);

        for (int n = 0; n < c; n++)
        {
            sum += Math.Pow(a, n) / Factorial(n);
        }

        double lastTerm = Math.Pow(a, c) / (Factorial(c) * (1 - rho));

        return 1.0 / (sum + lastTerm);
    }

    static double ConvertToMinutes(double value, int unit)
    {
        if (unit == 1) return value * 60;
        if (unit == 2) return value;
        if (unit == 3) return value / 60;
        return value;
    }

    static double ConvertFromMinutes(double value, int unit)
    {
        if (unit == 1) return value * 60;
        if (unit == 2) return value;
        if (unit == 3) return value / 60;
        return value;
    }

    static void Main()
    {
        Console.WriteLine("Select Time Unit:");
        Console.WriteLine("1. Seconds");
        Console.WriteLine("2. Minutes");
        Console.WriteLine("3. Hours");
        Console.Write("Enter choice: ");
        int unit = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter λ (arrival rate): ");
        double lambda = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter μ (service rate): ");
        double mu = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter number of servers (c): ");
        int c = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Cs (service variability): ");
        double Cs = Convert.ToDouble(Console.ReadLine());

        // Convert to per minute
        lambda = ConvertToMinutes(lambda, unit);
        mu = ConvertToMinutes(mu, unit);

        double rho = lambda / (c * mu);

        if (rho >= 1)
        {
            Console.WriteLine("\nSystem is UNSTABLE (ρ ≥ 1)");
            return;
        }

        // STEP 1: P0
        double P0 = CalculateP0(lambda, mu, c);

        // STEP 2: M/M/c Lq
        double a = lambda / mu;

        double Lq_mm = (P0 * Math.Pow(a, c) * rho) /
                       (Factorial(c) * Math.Pow(1 - rho, 2));

        // STEP 3: M/G/c (NO square on Cs)
        double Lq = Lq_mm * ((1 + Cs) / 2.0);

        // STEP 4: Performance Measures
        double Wq = Lq / lambda;
        double W = Wq + (1 / mu);
        double L = lambda * W;

        // Convert output
        double Wq_out = ConvertFromMinutes(Wq, unit);
        double W_out = ConvertFromMinutes(W, unit);

        // OUTPUT
        Console.WriteLine("\n===== Results (Modified M/G/c) =====");
        Console.WriteLine($"Utilization (ρ): {rho:F4}");
        Console.WriteLine($"P0: {P0:F4}");
        Console.WriteLine($"Lq: {Lq:F4}");
        Console.WriteLine($"Wq: {Wq_out:F4}");
        Console.WriteLine($"W: {W_out:F4}");
        Console.WriteLine($"L: {L:F4}");
    }
}
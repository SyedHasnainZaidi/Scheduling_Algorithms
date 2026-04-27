using System;

class GGCQueue
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

        for (int n = 0; n < c; n++)
        {
            sum += Math.Pow(a, n) / Factorial(n);
        }

        double rho = lambda / (c * mu);
        double lastTerm = Math.Pow(a, c) / (Factorial(c) * (1 - rho));

        return 1.0 / (sum + lastTerm);
    }

    static double ConvertToMinutes(double value, int unit)
    {
        // 1 = seconds, 2 = minutes, 3 = hours
        if (unit == 1) return value * 60;      // per second → per minute
        if (unit == 2) return value;           // already per minute
        if (unit == 3) return value / 60;      // per hour → per minute
        return value;
    }

    static double ConvertFromMinutes(double value, int unit)
    {
        if (unit == 1) return value * 60;      // minutes → seconds
        if (unit == 2) return value;           // minutes
        if (unit == 3) return value / 60;      // minutes → hours
        return value;
    }

    static void Main()
    {
        // UNIT SELECTION
        Console.WriteLine("Select Time Unit:");
        Console.WriteLine("1. Seconds");
        Console.WriteLine("2. Minutes");
        Console.WriteLine("3. Hours");
        Console.Write("Enter choice: ");
        int unit = Convert.ToInt32(Console.ReadLine());

        // USER INPUT
        Console.Write("Enter arrival rate (lambda): ");
        double lambda = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter service rate (mu): ");
        double mu = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter number of servers (c): ");
        int c = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Ca: ");
        double Ca = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter Cs: ");
        double Cs = Convert.ToDouble(Console.ReadLine());

        // Convert rates to per minute
        lambda = ConvertToMinutes(lambda, unit);
        mu = ConvertToMinutes(mu, unit);

        // STEP 1: Utilization
        double rho = lambda / (c * mu);

        if (rho >= 1)
        {
            Console.WriteLine("\nSystem is UNSTABLE (rho >= 1)");
            return;
        }

        // STEP 2: P0
        double P0 = CalculateP0(lambda, mu, c);

        // STEP 3: Lq (M/M/c)
        double a = lambda / mu;
        double Lq_mm = (P0 * Math.Pow(a, c) * rho) /
                       (Factorial(c) * Math.Pow(1 - rho, 2));

        // STEP 4: G/G/c adjustment
        double variabilityFactor = (Math.Pow(Ca, 2) + Math.Pow(Cs, 2)) / 2.0;
        double Lq = Lq_mm * variabilityFactor;

        // STEP 5: Times (in minutes)
        double Wq = Lq / lambda;
        double W = Wq + (1 / mu);

        // Convert time results back to user unit
        double Wq_out = ConvertFromMinutes(Wq, unit);
        double W_out = ConvertFromMinutes(W, unit);

        double L = lambda * W;

        // OUTPUT
        Console.WriteLine("\n===== Results =====");
        Console.WriteLine($"Utilization (rho): {rho:F4}");
        Console.WriteLine($"P0: {P0:F4}");
        Console.WriteLine($"Lq: {Lq:F4}");
        Console.WriteLine($"Wq: {Wq_out:F4}");
        Console.WriteLine($"W: {W_out:F4}");
        Console.WriteLine($"L: {L:F4}");
    }
}
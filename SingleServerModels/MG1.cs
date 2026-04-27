using System;

class MG1Queue
{
    static double ToPerMinute(double value, int unit)
    {
        if (unit == 1) return value * 60;   // sec → min
        if (unit == 2) return value;        // min
        if (unit == 3) return value / 60;   // hr → min
        return value;
    }

    static double FromMinutes(double value, int unit)
    {
        if (unit == 1) return value * 60;
        if (unit == 2) return value;
        if (unit == 3) return value / 60;
        return value;
    }

    static void Main()
    {
        // UNIT
        Console.WriteLine("Select Time Unit:");
        Console.WriteLine("1. Seconds");
        Console.WriteLine("2. Minutes");
        Console.WriteLine("3. Hours");
        int unit = Convert.ToInt32(Console.ReadLine());

        // ARRIVAL
        Console.Write("Enter λ (arrival rate): ");
        double lambda = Convert.ToDouble(Console.ReadLine());
        lambda = ToPerMinute(lambda, unit);

        // DISTRIBUTION CHOICE
        Console.WriteLine("\nSelect Service Distribution:");
        Console.WriteLine("1. Exponential (M/M/1)");
        Console.WriteLine("2. Uniform (a, b)");
        Console.WriteLine("3. General (mean & variance)");
        int choice = Convert.ToInt32(Console.ReadLine());

        double meanS = 0;
        double variance = 0;
        double mu = 0;

        if (choice == 1)
        {
            // EXPONENTIAL
            Console.Write("Enter μ (service rate): ");
            mu = Convert.ToDouble(Console.ReadLine());
            mu = ToPerMinute(mu, unit);

            meanS = 1 / mu;
            variance = 1 / (mu * mu);
        }
        else if (choice == 2)
        {
            // UNIFORM
            Console.Write("Enter a (min service time): ");
            double a = ToPerMinute(Convert.ToDouble(Console.ReadLine()), unit);

            Console.Write("Enter b (max service time): ");
            double b = ToPerMinute(Convert.ToDouble(Console.ReadLine()), unit);

            meanS = (a + b) / 2;
            variance = Math.Pow(b - a, 2) / 12;

            mu = 1 / meanS;
        }
        else if (choice == 3)
        {
            // GENERAL
            Console.Write("Enter mean service time: ");
            meanS = ToPerMinute(Convert.ToDouble(Console.ReadLine()), unit);

            Console.Write("Enter variance of service time: ");
            variance = Math.Pow(ToPerMinute(Math.Sqrt(Convert.ToDouble(Console.ReadLine())), unit), 2);

            mu = 1 / meanS;
        }
        else
        {
            Console.WriteLine("Invalid choice");
            return;
        }

        // TRAFFIC INTENSITY
        double rho = lambda / mu;

        if (rho >= 1)
        {
            Console.WriteLine("\nSystem is UNSTABLE (ρ ≥ 1)");
            return;
        }

        // Lq
        double Lq = (Math.Pow(lambda, 2) * variance + Math.Pow(rho, 2))
                    / (2 * (1 - rho));

        // Wq
        double Wq = Lq / lambda;

        // W
        double W = Wq + meanS;

        // L
        double L = lambda * W;

        // Idle Probability
        double P0 = 1 - rho;

        // Convert output
        double Wq_out = FromMinutes(Wq, unit);
        double W_out = FromMinutes(W, unit);

        // OUTPUT
        Console.WriteLine("\n===== M/G/1 Results =====");
        Console.WriteLine($"ρ: {rho:F4}");
        Console.WriteLine($"P0: {P0:F4}");
        Console.WriteLine($"Mean Service Time: {meanS:F4}");
        Console.WriteLine($"Variance: {variance:F4}");
        Console.WriteLine($"Lq: {Lq:F4}");
        Console.WriteLine($"Wq: {Wq_out:F4}");
        Console.WriteLine($"W: {W_out:F4}");
        Console.WriteLine($"L: {L:F4}");
    }
}
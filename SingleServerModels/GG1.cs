using System;

class GG1Queue
{
    // Convert rates to per minute
    static double ConvertToMinutes(double value, int unit)
    {
        if (unit == 1) return value / 60;   // per hour → per minute
        if (unit == 2) return value;        // per minute
        if (unit == 3) return value * 60;   // per second → per minute
        return value;
    }

    static void Main()
    {
        // 🔹 STEP 0: UNIT SELECTION
        Console.WriteLine("Select Time Unit:");
        Console.WriteLine("1. Hours");
        Console.WriteLine("2. Minutes");
        Console.WriteLine("3. Seconds");
        Console.Write("Enter choice: ");
        int unit = Convert.ToInt32(Console.ReadLine());

        // 🔹 INPUT
        Console.Write("Enter λ (arrival rate): ");
        double lambda = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter μ (service rate): ");
        double mu = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter variance of ARRIVAL (σa^2): ");
        double varArrival = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter variance of SERVICE (σs^2): ");
        double varService = Convert.ToDouble(Console.ReadLine());

        // Convert rates
        lambda = ConvertToMinutes(lambda, unit);
        mu = ConvertToMinutes(mu, unit);

        // 🔹 STEP 1: Traffic Intensity
        double rho = lambda / mu;

        if (rho >= 1)
        {
            Console.WriteLine("\nSystem is UNSTABLE (ρ ≥ 1)");
            return;
        }

        // 🔹 STEP 2: Ca^2 and Cs^2
        double meanInterarrival = 1 / lambda;
        double meanService = 1 / mu;

        double Ca2 = varArrival / Math.Pow(meanInterarrival, 2);
        double Cs2 = varService / Math.Pow(meanService, 2);

        // 🔹 STEP 3: Lq (G/G/1 formula)
        double rho2 = Math.Pow(rho, 2);

        double numerator = rho2 * (1 + Cs2) * (Ca2 + rho2 * Cs2);
        double denominator = 2 * (1 - rho) * (1 + rho2 * Cs2);

        double Lq = numerator / denominator;

        // 🔹 STEP 4: Wq
        double Wq = Lq / lambda;

        // 🔹 STEP 5: W
        double W = Wq + meanService;

        // 🔹 STEP 6: L
        double L = lambda * W;

        // 🔹 STEP 7: P0
        double P0 = 1 - rho;

        // OUTPUT
        Console.WriteLine("\n===== G/G/1 Results =====");
        Console.WriteLine($"Traffic Intensity (ρ): {rho:F4}");
        Console.WriteLine($"Ca^2: {Ca2:F4}");
        Console.WriteLine($"Cs^2: {Cs2:F4}");
        Console.WriteLine($"Lq: {Lq:F4}");
        Console.WriteLine($"Wq: {Wq:F4}");
        Console.WriteLine($"W: {W:F4}");
        Console.WriteLine($"L: {L:F4}");
        Console.WriteLine($"P0: {P0:F4}");
    }
}
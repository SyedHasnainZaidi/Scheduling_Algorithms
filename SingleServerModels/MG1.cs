using System;

namespace SingleServerModels
{
    public class MG1
    {
        // Helper methods for unit conversion
        private static double ToHours(double value, string unit)
        {
            switch (unit.ToLower())
            {
                case "sec":
                case "s":
                    return value / 3600.0;
                case "min":
                case "m":
                    return value / 60.0;
                case "hr":
                case "h":
                    return value;
                default:
                    return value; // default assume hours
            }
        }

        private static double FromHours(double value, string unit)
        {
            switch (unit.ToLower())
            {
                case "sec":
                case "s":
                    return value * 3600.0;
                case "min":
                case "m":
                    return value * 60.0;
                case "hr":
                case "h":
                    return value;
                default:
                    return value; // default assume hours
            }
        }

        public static void Solve()
        {
            Console.WriteLine("\n--- M/G/1 Model ---");

            // Ask user for unit
            Console.Write("Select time unit (sec/min/hr): ");
            string unit = Console.ReadLine();

            // Input rates in chosen unit
            Console.Write($"Enter Arrival Rate (λ) per {unit}: ");
            double lambda = Convert.ToDouble(Console.ReadLine());
            Console.Write($"Enter Service Rate (μ) per {unit}: ");
            double mu = Convert.ToDouble(Console.ReadLine());
            Console.Write("Enter Service Time Variance (σ²) in hours²: ");
            double variance = Convert.ToDouble(Console.ReadLine());

            // Convert rates to hours internally
            lambda = ToHours(lambda, unit);
            mu = ToHours(mu, unit);

            double rho = lambda / mu;

            if (rho >= 1)
            {
                Console.WriteLine("System is Unstable!");
                return;
            }

            double Wq = (lambda * variance + (rho * rho)) / (2 * (1 - rho));
            double Lq = lambda * Wq;
            double W = Wq + (1 / mu);
            double L = lambda * W;

            // Convert W and Wq back to chosen unit
            W = FromHours(W, unit);
            Wq = FromHours(Wq, unit);

            Console.WriteLine("\n--- Results ---");
            Console.WriteLine("Utilization (ρ): " + Math.Round(rho, 3));
            Console.WriteLine("L  : " + Math.Round(L, 3));
            Console.WriteLine("Lq : " + Math.Round(Lq, 3));
            Console.WriteLine("W  : " + Math.Round(W, 3) + " " + unit);
            Console.WriteLine("Wq : " + Math.Round(Wq, 3) + " " + unit);
        }
    }
}
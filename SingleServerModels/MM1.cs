using System;

namespace SingleServerModels
{
    public class MM1
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
                    return value; // assume hours by default
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
                    return value; // assume hours by default
            }
        }

        public static void Solve()
        {
            Console.WriteLine("\n--- M/M/1 Model ---");

            // Ask user for unit
            Console.Write("Select time unit (sec/min/hr): ");
            string unit = Console.ReadLine();

            // Input rates in chosen unit
            Console.Write($"Enter Arrival Rate (λ) per {unit}: ");
            double lambda = Convert.ToDouble(Console.ReadLine());
            Console.Write($"Enter Service Rate (μ) per {unit}: ");
            double mu = Convert.ToDouble(Console.ReadLine());

            // Convert rates to hours for internal calculation
            lambda = ToHours(lambda, unit);
            mu = ToHours(mu, unit);

            if (lambda >= mu)
            {
                Console.WriteLine("System is Unstable!");
                return;
            }

            double rho = lambda / mu;
            double P0 = 1 - rho;
            double L = lambda / (mu - lambda);
            double Lq = (lambda * lambda) / (mu * (mu - lambda));
            double W = 1 / (mu - lambda);
            double Wq = lambda / (mu * (mu - lambda));

            // Convert W and Wq back to chosen unit
            W = FromHours(W, unit);
            Wq = FromHours(Wq, unit);

            Console.WriteLine($"ρ = {Math.Round(rho,3)}, P0 = {Math.Round(P0,3)}, L = {Math.Round(L,3)}, Lq = {Math.Round(Lq,3)}, W = {Math.Round(W,3)} {unit}, Wq = {Math.Round(Wq,3)} {unit}");
        }
    }
}
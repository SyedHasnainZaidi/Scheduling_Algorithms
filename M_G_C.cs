using System;

namespace doubleServerModels
{
    public class MGC
    {
        private static double ToHours(double value, string unit)
        {
            switch (unit.ToLower())
            {
                case "sec":
                case "s": return value / 3600.0;
                case "min":
                case "m": return value / 60.0;
                case "hr":
                case "h": return value;
                default: return value;
            }
        }

        private static double FromHours(double value, string unit)
        {
            switch (unit.ToLower())
            {
                case "sec":
                case "s": return value * 3600.0;
                case "min":
                case "m": return value * 60.0;
                case "hr":
                case "h": return value;
                default: return value;
            }
        }

        public static void Solve()
        {
            Console.WriteLine("\n--- M/G/c Model ---");

            Console.Write("Select time unit (sec/min/hr): ");
            string unit = Console.ReadLine() ?? "";

            Console.Write($"Enter Arrival Rate (λ) per {unit}: ");
            double lambda = Convert.ToDouble(Console.ReadLine());

            Console.Write($"Enter Service Rate (μ) per {unit}: ");
            double mu = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter number of servers (c): ");
            int c = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter Cs^2: ");
            double Cs2 = Convert.ToDouble(Console.ReadLine());

            lambda = ToHours(lambda, unit);
            mu = ToHours(mu, unit);

            double rho = lambda / (c * mu);

            if (rho >= 1)
            {
                Console.WriteLine("System is Unstable!");
                return;
            }

            double exponent = Math.Sqrt(2 * (c + 1)) - 1;

            double Wq = ((Cs2 + 1) / 2.0) *
                        (Math.Pow(rho, exponent) / (c * (1 - rho))) *
                        (1 / mu);

            double Lq = lambda * Wq;
            double W = Wq + (1 / mu);
            double L = lambda * W;

            W = FromHours(W, unit);
            Wq = FromHours(Wq, unit);

            Console.WriteLine($"ρ = {Math.Round(rho,3)}, L = {Math.Round(L,3)}, Lq = {Math.Round(Lq,3)}, W = {Math.Round(W,3)} {unit}, Wq = {Math.Round(Wq,3)} {unit}");
        }
    }
}
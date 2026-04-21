using System;

namespace doubleServerModels
{
    public class MMS
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

        static double Factorial(int n)
        {
            double fact = 1;
            for (int i = 1; i <= n; i++)
                fact *= i;
            return fact;
        }

        public static void Solve()
        {
            Console.WriteLine("\n--- M/M/s Model ---");

            Console.Write("Select time unit (sec/min/hr): ");
            string unit = Console.ReadLine() ?? "";

            Console.Write($"Enter Arrival Rate (λ) per {unit}: ");
            double lambda = Convert.ToDouble(Console.ReadLine());

            Console.Write($"Enter Service Rate (μ) per {unit}: ");
            double mu = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter number of servers (s): ");
            int s = Convert.ToInt32(Console.ReadLine());

            lambda = ToHours(lambda, unit);
            mu = ToHours(mu, unit);

            double rho = lambda / (s * mu);

            if (rho >= 1)
            {
                Console.WriteLine("System is Unstable!");
                return;
            }

            double sum = 0;
            for (int n = 0; n < s; n++)
                sum += Math.Pow(lambda / mu, n) / Factorial(n);

            double lastTerm = Math.Pow(lambda / mu, s) /
                              (Factorial(s) * (1 - rho));

            double P0 = 1 / (sum + lastTerm);

            double Lq = (P0 * Math.Pow(lambda / mu, s) * rho) /
                        (Factorial(s) * Math.Pow(1 - rho, 2));

            double Wq = Lq / lambda;
            double W = Wq + (1 / mu);
            double L = lambda * W;

            W = FromHours(W, unit);
            Wq = FromHours(Wq, unit);

            Console.WriteLine($"ρ = {Math.Round(rho,3)}, L = {Math.Round(L,3)}, Lq = {Math.Round(Lq,3)}, W = {Math.Round(W,3)} {unit}, Wq = {Math.Round(Wq,3)} {unit}");
        }
    }
}
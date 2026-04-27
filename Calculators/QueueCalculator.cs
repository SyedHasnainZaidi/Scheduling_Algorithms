public static class QueueCalculator
{
    public static double ToPerHourRate(double value, string? unit)
    {
        return QueueUnitConverter.ToPerHourRate(value, unit);
    }

    public static double ToHoursSquared(double value, string? unit)
    {
        return QueueUnitConverter.ToHoursSquared(value, unit);
    }

    public static CalculationResponse CalculateMm1(double lambda, double mu, string unit)
    {
        return MM1ModelCalculator.Calculate(lambda, mu, unit);
    }

    public static CalculationResponse CalculateMg1(double lambda, double mu, double varianceHoursSquared, string unit)
    {
        return MG1ModelCalculator.Calculate(lambda, mu, varianceHoursSquared, unit);
    }

    public static CalculationResponse CalculateGg1(double lambda, double mu, double ca, double cs, string unit)
    {
        return GG1ModelCalculator.Calculate(lambda, mu, ca, cs, unit);
    }

    public static CalculationResponse CalculateMms(double lambda, double mu, int servers, string unit)
    {
        return MMSModelCalculator.Calculate(lambda, mu, servers, unit);
    }

    public static CalculationResponse CalculateMgc(double lambda, double mu, int servers, double cs2, string unit)
    {
        return MGCModelCalculator.Calculate(lambda, mu, servers, cs2, unit);
    }

    public static CalculationResponse CalculateGgc(double lambda, double mu, int servers, double ca, double cs, string unit)
    {
        return GGCModelCalculator.Calculate(lambda, mu, servers, ca, cs, unit);
    }
}

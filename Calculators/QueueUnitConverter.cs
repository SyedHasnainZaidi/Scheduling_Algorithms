public static class QueueUnitConverter
{
    public static double ToPerHourRate(double value, string? unit)
    {
        var normalized = unit?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "sec" or "s" => value * 3600.0,
            "min" or "m" => value * 60.0,
            "hr" or "h" => value,
            _ => value
        };
    }

    public static double ToHoursSquared(double value, string? unit)
    {
        var normalized = unit?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "sec" or "s" => value / (3600.0 * 3600.0),
            "min" or "m" => value / (60.0 * 60.0),
            "hr" or "h" => value,
            _ => value
        };
    }

    public static double FromHours(double value, string? unit)
    {
        var normalized = unit?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "sec" or "s" => value * 3600.0,
            "min" or "m" => value * 60.0,
            "hr" or "h" => value,
            _ => value
        };
    }

    public static double ToHours(double value, string? unit)
    {
        var normalized = unit?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "sec" or "s" => value / 3600.0,
            "min" or "m" => value / 60.0,
            "hr" or "h" => value,
            _ => value
        };
    }

    public static double FromHoursSquared(double value, string? unit)
    {
        var normalized = unit?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "sec" or "s" => value * 3600.0 * 3600.0,
            "min" or "m" => value * 60.0 * 60.0,
            "hr" or "h" => value,
            _ => value
        };
    }
}

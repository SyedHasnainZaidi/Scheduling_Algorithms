namespace SimulatorFinalProject.Simulator.Models;

public class SimulationRequest
{
    public int NumberOfCustomers { get; set; } = 400;
    public double ArrivalRatePerHour { get; set; } = 12.28;
    public double ServiceRatePerHour { get; set; } = 6.86;
    public int NumberOfServers { get; set; } = 3;
}

public class SimulationResponse
{
    public SimulationSummary Summary { get; set; } = new();
    public List<CustomerSimulationResult> Customers { get; set; } = [];
}

public class SimulationSummary
{
    public int NumberOfCustomers { get; set; }
    public int NumberOfServers { get; set; }
    public double ArrivalRatePerHour { get; set; }
    public double ServiceRatePerHour { get; set; }
    public double AverageWaitingTimeMinutes { get; set; }
    public double AverageSystemTimeMinutes { get; set; }
    public double AverageQueueLengthAtArrival { get; set; }
    public double ServerUtilization { get; set; }
    public double TotalSimulatedTimeHours { get; set; }
}

public class CustomerSimulationResult
{
    public int CustomerId { get; set; }
    public int AssignedServer { get; set; }
    public double InterArrivalTimeMinutes { get; set; }
    public double ArrivalTimeMinutes { get; set; }
    public double ServiceStartTimeMinutes { get; set; }
    public double WaitingTimeMinutes { get; set; }
    public double ServiceTimeMinutes { get; set; }
    public double DepartureTimeMinutes { get; set; }
    public int QueueLengthAtArrival { get; set; }
}

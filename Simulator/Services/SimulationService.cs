using SimulatorFinalProject.Simulator.Models;

namespace SimulatorFinalProject.Simulator.Services;

public class SimulationService
{
    private const long Seed = 123;
    private const long Modulus = 4294967296;
    private const long Multiplier = 1664525;
    private const long Increment = 1013904223;

    public SimulationResponse Run(SimulationRequest request)
    {
        ValidateRequest(request);

        var lcg = new LcgRandom(Seed, Multiplier, Increment, Modulus);

        var nextAvailableTimeByServer = new double[request.NumberOfServers];
        var departures = new List<double>(request.NumberOfCustomers);
        var results = new List<CustomerSimulationResult>(request.NumberOfCustomers);
        double cumulativeArrival = 0.0;
        double totalWaiting = 0.0;
        double totalSystem = 0.0;
        double totalBusyTime = 0.0;
        double totalQueueLengthAtArrival = 0.0;

        for (var customerIndex = 0; customerIndex < request.NumberOfCustomers; customerIndex++)
        {
            var interArrival = SampleExponential(request.ArrivalRatePerHour, lcg);
            if (customerIndex == 0)
            {
                interArrival = 0.0;
            }

            cumulativeArrival += interArrival;
            var queueLengthAtArrival = CountQueueLengthAtArrival(cumulativeArrival, departures, nextAvailableTimeByServer);

            var assignedServer = 0;
            var earliestServerTime = nextAvailableTimeByServer[0];
            for (var serverIndex = 1; serverIndex < nextAvailableTimeByServer.Length; serverIndex++)
            {
                if (nextAvailableTimeByServer[serverIndex] < earliestServerTime)
                {
                    earliestServerTime = nextAvailableTimeByServer[serverIndex];
                    assignedServer = serverIndex;
                }
            }

            var serviceStart = Math.Max(cumulativeArrival, earliestServerTime);
            var wait = serviceStart - cumulativeArrival;
            var serviceTime = SampleExponential(request.ServiceRatePerHour, lcg);
            var departure = serviceStart + serviceTime;

            nextAvailableTimeByServer[assignedServer] = departure;
            departures.Add(departure);

            totalWaiting += wait;
            totalSystem += wait + serviceTime;
            totalBusyTime += serviceTime;
            totalQueueLengthAtArrival += queueLengthAtArrival;

            results.Add(new CustomerSimulationResult
            {
                CustomerId = customerIndex + 1,
                AssignedServer = assignedServer + 1,
                InterArrivalTimeMinutes = RoundMinutes(interArrival),
                ArrivalTimeMinutes = RoundMinutes(cumulativeArrival),
                ServiceStartTimeMinutes = RoundMinutes(serviceStart),
                WaitingTimeMinutes = RoundMinutes(wait),
                ServiceTimeMinutes = RoundMinutes(serviceTime),
                DepartureTimeMinutes = RoundMinutes(departure),
                QueueLengthAtArrival = queueLengthAtArrival
            });
        }

        var totalTime = results.Count == 0 ? 0.0 : results[^1].DepartureTimeMinutes / 60.0;
        var utilization = totalTime <= 0.0 ? 0.0 : totalBusyTime / (request.NumberOfServers * totalTime);

        return new SimulationResponse
        {
            Summary = new SimulationSummary
            {
                NumberOfCustomers = request.NumberOfCustomers,
                NumberOfServers = request.NumberOfServers,
                ArrivalRatePerHour = request.ArrivalRatePerHour,
                ServiceRatePerHour = request.ServiceRatePerHour,
                AverageWaitingTimeMinutes = Math.Round(totalWaiting / request.NumberOfCustomers * 60.0, 4),
                AverageSystemTimeMinutes = Math.Round(totalSystem / request.NumberOfCustomers * 60.0, 4),
                AverageQueueLengthAtArrival = Math.Round(totalQueueLengthAtArrival / request.NumberOfCustomers, 4),
                ServerUtilization = Math.Round(utilization, 4),
                TotalSimulatedTimeHours = Math.Round(totalTime, 4)
            },
            Customers = results
        };
    }

    private static int CountQueueLengthAtArrival(
        double arrivalTimeHours,
        List<double> departures,
        double[] nextAvailableTimeByServer)
    {
        var inSystem = 0;
        for (var i = 0; i < departures.Count; i++)
        {
            if (departures[i] > arrivalTimeHours)
            {
                inSystem++;
            }
        }

        var busyServers = 0;
        for (var i = 0; i < nextAvailableTimeByServer.Length; i++)
        {
            if (nextAvailableTimeByServer[i] > arrivalTimeHours)
            {
                busyServers++;
            }
        }

        return Math.Max(0, inSystem - busyServers);
    }

    private static void ValidateRequest(SimulationRequest request)
    {
        if (request.NumberOfCustomers <= 0)
        {
            throw new ArgumentException("Number of customers must be greater than 0.");
        }

        if (request.ArrivalRatePerHour <= 0 || request.ServiceRatePerHour <= 0)
        {
            throw new ArgumentException("Arrival rate and service rate must be greater than 0.");
        }

        if (request.NumberOfServers <= 0)
        {
            throw new ArgumentException("Number of servers must be greater than 0.");
        }

        if (request.ArrivalRatePerHour >= request.NumberOfServers * request.ServiceRatePerHour)
        {
            throw new ArgumentException("System is unstable. Ensure arrival rate is less than servers multiplied by service rate.");
        }
    }

    private static double SampleExponential(double ratePerHour, LcgRandom lcg)
    {
        var u = lcg.NextUnit();
        if (u <= 0.0)
        {
            u = 1.0 / lcg.Modulus;
        }

        return -Math.Log(1.0 - u) / ratePerHour;
    }

    private static double RoundMinutes(double hours) => Math.Round(hours * 60.0, 4);

    private sealed class LcgRandom
    {
        private ulong _state;

        public LcgRandom(long seed, long multiplier, long increment, long modulus)
        {
            _state = (ulong)seed;
            Multiplier = (ulong)multiplier;
            Increment = (ulong)increment;
            Modulus = (ulong)modulus;
        }

        public ulong Modulus { get; }
        private ulong Multiplier { get; }
        private ulong Increment { get; }

        public double NextUnit()
        {
            _state = (Multiplier * _state + Increment) % Modulus;
            return (double)_state / Modulus;
        }
    }
}

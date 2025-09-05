using System;
using System.Collections.Generic;
using System.Linq;
using ElevatorApp.Domain;
using ElevatorApp.Domain.Enums;
using System.Threading.Tasks;

namespace ElevatorApp.Application
{
    /// <summary>
    /// Coordinates a fleet of elevators:
    /// - Greedy, proximity-based dispatch.
    /// - Splits large passenger groups across multiple elevators when needed.
    /// - Queues any remainder if total capacity is insufficient.
    ///
    /// Design notes (POC-aligned):
    /// - SRP: Controller only orchestrates; elevators handle movement/load.
    /// - OCP: Dispatch strategy can be swapped/extended without touching callers.
    /// - DIP: Depends on elevator abstraction (domain), not concrete UI.
    /// </summary>
    public class ElevatorController
    {
        private readonly List<ElevatorBase> _elevators;
        private readonly List<ElevatorRequest> _pendingRequests;
        private int _passengerCounter = 0; // simple ID generator for demo passengers

        public ElevatorController(List<ElevatorBase> elevators)
        {
            _elevators = elevators ?? throw new ArgumentNullException(nameof(elevators));
            _pendingRequests = new List<ElevatorRequest>();
        }

        /// <summary>Snapshot for UI/tests.</summary>
        public IReadOnlyList<ElevatorBase> GetElevators() => _elevators.AsReadOnly();

        /// <summary>Total passengers still queued across all floors.</summary>
        public int PendingPassengers => _pendingRequests.Sum(r => r.PassengerCount);

        /// <summary>Places a new request; tries to serve immediately; queues any shortfall.</summary>
    
        public void RequestElevator(int floor, int floorto , int passengerCount)
        {
            // Select closest elevator
            var chosenElevator = SelectBestElevator(floor, floorto);

            // Add request and passengers
            // Add request and passengers
            switch (chosenElevator)
            {
                case PassengerElevator pe:
                    pe.AddRequest(floor, passengerCount);
                    break;
                case FreightElevator fe:
                    // Treat passengerCount as "load units" for freight requests (configurable)
                    fe.AddFreightRequest(floor, passengerCount);
                    break;
                default:
                    Console.WriteLine($"[Controller] Elevator {chosenElevator.Id} cannot accept this request type.");
                    break;
            }
        }
       
        private ElevatorBase SelectBestElevator(int floor,int floorto )
        {
            // SCAN-like heuristic: prefer elevators that are moving towards the request (and will pass it),
            // then idle nearest cars, then moving-away cars by estimated distance.
            var scores = new List<(ElevatorBase e, double score)>();

            //using the 2 inputs we get direction the passenger are supposedly heading
            var requestedDirection = floorto > floor ? Direction.Up : Direction.Down;

                foreach (var e in _elevators)
                {
                    double score = 0;
                    int dist = Math.Abs(e.CurrentFloor - floor);

                    // Case 1: elevator moving in same direction as request AND will pass pickup
                    if (e.Direction == requestedDirection)
                    {
                        if ((requestedDirection == Direction.Up && e.CurrentFloor <= floor) ||
                            (requestedDirection == Direction.Down && e.CurrentFloor >= floor))
                        {
                            score -= 200 - dist; // strong preference
                        }
                        else
                        {
                            score += 50 + dist; // going wrong way, but same direction eventually
                        }
                    }
                    // Case 2: elevator is idle
                    else if (e.Direction == Direction.Idle)
                    {
                        score -= 100 - dist; // idle elevators: prefer closer ones
                    }
                    // Case 3: elevator moving opposite direction
                    else
                    {
                        score += 200 + dist; // penalize moving away
                    }

                    // Small bonus for emptier elevators (avoid overloading)
                    score += (e.Capacity - e.Passengers.Count) * -0.5;

                    scores.Add((e, score));
                }

                // lowest score = best candidate
                return scores.OrderBy(s => s.score).First().e;
            }



            /// <summary>Re-attempts queued requests using the current state of the fleet.</summary>
        public void ProcessPendingRequests()
        {
            if (_pendingRequests.Count == 0) return;

            var stillPending = new List<ElevatorRequest>();

            foreach (var req in _pendingRequests)
            {
                var left = ServeRequest(req);
                if (left > 0)
                {
                    // keep the remainder in queue
                    stillPending.Add(new ElevatorRequest(req.FloorNumber,req.FloortoNumber, left));
                }
            }

            _pendingRequests.Clear();
            _pendingRequests.AddRange(stillPending);
        }

        /// <summary>Console-friendly status printout.</summary>
        public void PrintElevatorStatus()
        {
            Console.WriteLine("\n=== Elevator Status ===");
            foreach (var e in _elevators)
            {
                Console.WriteLine($"Elevator {e.Id}: Floor {e.CurrentFloor}, " +
                                  $"Direction: {e.Direction}, State: {e.State}, " +
                                  $"Passengers: {e.Passengers.Count}/{e.Capacity}");
            }

            if (_pendingRequests.Count > 0)
            {
                var total = _pendingRequests.Sum(r => r.PassengerCount);
                Console.WriteLine($"Pending (queued) passengers: {total}");
            }
            Console.WriteLine("========================\n");
        }

        // -------------------------
        // Internals (strategy bits)
        // -------------------------

        /// <summary>
        /// Core strategy: serve a request by using *several* nearest elevators in order,
        /// loading as many as possible on each, until everyone is served or we run out of capacity.
        ///
        /// Returns the number of passengers that couldn't be served right now.
        /// </summary>
        private int ServeRequest(ElevatorRequest request)
        {
            var remaining = request.PassengerCount;

            // Choose candidates by proximity, then by how empty they are (tie-breaker)
            var candidates = _elevators
                .OrderBy(e => Math.Abs(e.CurrentFloor - request.FloorNumber))
                .ThenByDescending(e => e.AvailableCapacity) // prefer emptier cars
                .ToList();

            foreach (var elevator in candidates)
            {
                if (remaining <= 0) break;

                // Skip cars with no space at all
                if (elevator.AvailableCapacity <= 0) continue;

                Console.WriteLine($"[Controller] Dispatching Elevator {elevator.Id} to Floor {request.FloorNumber} to carry passengers to {request.FloortoNumber}");
                elevator.MoveTo(request.FloorNumber ,request.FloortoNumber);

                // Free space if anyone wants this floor
                elevator.UnloadPassengersAtCurrentFloor();

                var space = elevator.AvailableCapacity;
                if (space <= 0) continue;

                var toLoad = Math.Min(space, remaining);
                Console.WriteLine($"{space} places avilable in elevator sent to to Floor {request.FloorNumber}. {remaining} passengers remaining");

                // Demo: destinations are "next floor up" to keep the POC simple/testable.
                for (int i = 0; i < toLoad; i++)
                {
                    elevator.LoadPassenger(new Passenger(NextPassengerId(), request.FloorNumber + 1));

                    //keep moving till the lift gets to the floor
                    do
                    {
                        elevator.MoveTo(request.FloorNumber ,request.FloortoNumber);

                    } while (elevator.CurrentFloor != request.FloortoNumber);
                }

                remaining -= toLoad;
            }

            return remaining;
        }

        private int NextPassengerId() => ++_passengerCounter;
    }
}

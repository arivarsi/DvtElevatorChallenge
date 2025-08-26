using System;
using System.Collections.Generic;
using System.Linq;
using ElevatorApp.Domain;
using ElevatorApp.Domain.Enums;

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
        private readonly List<Elevator> _elevators;
        private readonly List<ElevatorRequest> _pendingRequests;
        private int _passengerCounter = 0; // simple ID generator for demo passengers

        public ElevatorController(List<Elevator> elevators)
        {
            _elevators = elevators ?? throw new ArgumentNullException(nameof(elevators));
            _pendingRequests = new List<ElevatorRequest>();
        }

        /// <summary>Snapshot for UI/tests.</summary>
        public IReadOnlyList<Elevator> GetElevators() => _elevators.AsReadOnly();

        /// <summary>Total passengers still queued across all floors.</summary>
        public int PendingPassengers => _pendingRequests.Sum(r => r.PassengerCount);

        /// <summary>Places a new request; tries to serve immediately; queues any shortfall.</summary>
        public void RequestElevator(int floor, int passengerCount)
        {
            if (passengerCount <= 0)
            {
                Console.WriteLine("[Controller] Ignored non-positive passenger count.");
                return;
            }

            var request = new ElevatorRequest(floor, passengerCount);
            var remaining = ServeRequest(request);

            if (remaining > 0)
            {
                Console.WriteLine($"[Controller] Not enough capacity now. Queuing {remaining} passenger(s) at floor {floor}.");
                _pendingRequests.Add(new ElevatorRequest(floor, remaining));
            }
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
                    stillPending.Add(new ElevatorRequest(req.FloorNumber, left));
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

                Console.WriteLine($"[Controller] Dispatching Elevator {elevator.Id} to Floor {request.FloorNumber}");
                elevator.MoveTo(request.FloorNumber);

                // Free space if anyone wants this floor
                elevator.UnloadPassengersAtCurrentFloor();

                var space = elevator.AvailableCapacity;
                if (space <= 0) continue;

                var toLoad = Math.Min(space, remaining);

                // Demo: destinations are "next floor up" to keep the POC simple/testable.
                for (int i = 0; i < toLoad; i++)
                {
                    elevator.LoadPassenger(new Passenger(NextPassengerId(), request.FloorNumber + 1));
                }

                remaining -= toLoad;
            }

            return remaining;
        }

        private int NextPassengerId() => ++_passengerCounter;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ElevatorApp.Domain;
using ElevatorApp.Domain.Enums;

namespace ElevatorApp.Application
{
    /// <summary>
    /// Central controller responsible for coordinating multiple elevators.
    /// - Receives elevator requests
    /// - Selects the best available elevator (nearest + not full)
    /// - Handles edge cases (all busy or overloaded)
    /// </summary>
    public class ElevatorController
    {
        private readonly List<Elevator> _elevators;
        private readonly List<ElevatorRequest> _pendingRequests;

        public ElevatorController(List<Elevator> elevators)
        {
            _elevators = elevators ?? throw new ArgumentNullException(nameof(elevators));
            _pendingRequests = new List<ElevatorRequest>();
        }

        /// <summary>
        /// Handles an elevator request. 
        /// If an elevator is available, dispatch it immediately.
        /// Otherwise, queue the request for later processing.
        /// </summary>
        public void RequestElevator(int floor, int passengerCount)
        {
            var request = new ElevatorRequest(floor, passengerCount);

            var elevator = FindNearestAvailableElevator(floor, passengerCount);

            if (elevator != null)
            {
                Console.WriteLine($"[Controller] Dispatching Elevator {elevator.Id} to Floor {floor}");
                elevator.MoveTo(floor);
            }
            else
            {
                Console.WriteLine($"[Controller] No elevator available. Queuing request for Floor {floor}.");
                _pendingRequests.Add(request);
            }
        }

        /// <summary>
        /// Finds the nearest available elevator that has enough capacity.
        /// Implements a greedy strategy: choose elevator with the smallest distance to the requested floor.
        /// </summary>
        private Elevator? FindNearestAvailableElevator(int floor, int passengerCount)
        {
            return _elevators
                .Where(e => !e.IsFull() && (e.Capacity - e.Passengers.Count) >= passengerCount)
                .OrderBy(e => Math.Abs(e.CurrentFloor - floor))
                .FirstOrDefault();
        }

        /// <summary>
        /// Processes pending requests in case elevators have become available.
        /// Should be called periodically or when elevators finish tasks.
        /// </summary>
        public void ProcessPendingRequests()
        {
            var handledRequests = new List<ElevatorRequest>();

            foreach (var request in _pendingRequests)
            {
                var elevator = FindNearestAvailableElevator(request.FloorNumber, request.PassengerCount);

                if (elevator != null)
                {
                    Console.WriteLine($"[Controller] Processing queued request: Dispatching Elevator {elevator.Id} to Floor {request.FloorNumber}");
                    elevator.MoveTo(request.FloorNumber);
                    handledRequests.Add(request);
                }
            }

            // Remove completed requests from queue
            foreach (var r in handledRequests)
            {
                _pendingRequests.Remove(r);
            }
        }

        /// <summary>
        /// Provides a real-time snapshot of all elevator statuses.
        /// </summary>
        public void PrintElevatorStatus()
        {
            Console.WriteLine("\n=== Elevator Status ===");
            foreach (var e in _elevators)
            {
                Console.WriteLine($"Elevator {e.Id}: Floor {e.CurrentFloor}, " +
                                  $"Direction: {e.Direction}, State: {e.State}, " +
                                  $"Passengers: {e.Passengers.Count}/{e.Capacity}");
            }
            Console.WriteLine("========================\n");
        }
    }
}

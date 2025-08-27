using System;
using System.Threading;
using ElevatorApp.Domain.Enums;

namespace ElevatorApp.Domain
{
    /// <summary>
    /// Standard passenger elevator implementation.
    /// Moves floor-by-floor with delay, enforces normal passenger rules.
    /// </summary>
    public class PassengerElevator : ElevatorBase
    {
        public PassengerElevator(int id, int capacity, int startFloor = 0)
            : base(id, capacity, startFloor) { }

        public override void MoveTo(int targetFloor)
        {
            if (targetFloor == CurrentFloor)
            {
                Console.WriteLine($"[PassengerElevator {Id}] Already at floor {CurrentFloor}.");
                return;
            }

            Direction = targetFloor > CurrentFloor ? Direction.Up : Direction.Down;
            State = ElevatorState.Moving;

            Console.WriteLine($"[PassengerElevator {Id}] Starting at floor {CurrentFloor}, moving {Direction} to floor {targetFloor}...");

            while (CurrentFloor != targetFloor)
            {
                Thread.Sleep(500); // simulate delay
                CurrentFloor += (Direction == Direction.Up) ? 1 : -1;
                Console.WriteLine($"[PassengerElevator {Id}] Now at floor {CurrentFloor}...");
            }

            Stop();
            Console.WriteLine($"[PassengerElevator {Id}] Arrived at floor {CurrentFloor}.");
        }

        public override void Stop()
        {
            Direction = Direction.Idle;
            State = ElevatorState.Stationary;
        }

        public override void LoadPassenger(Passenger passenger)
        {
            if (!IsFull())
            {
                Passengers.Add(passenger);
                State = ElevatorState.Loading;
                Console.WriteLine($"[PassengerElevator {Id}] Passenger added (dest: {passenger.DestinationFloor}).");
            }
            else
            {
                Console.WriteLine($"[PassengerElevator {Id}] Cannot load passenger. Capacity reached!");
            }
        }

        public override void UnloadPassengersAtCurrentFloor()
        {
            var offloading = Passengers.RemoveAll(p => p.DestinationFloor == CurrentFloor);
            if (offloading > 0)
            {
                Console.WriteLine($"[PassengerElevator {Id}] {offloading} passenger(s) exited at floor {CurrentFloor}.");
                State = ElevatorState.Unloading;
            }
        }
    }
}

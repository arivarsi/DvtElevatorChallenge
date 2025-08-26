using System;
using System.Collections.Generic;
using System.Threading;
using ElevatorApp.Domain.Enums;

namespace ElevatorApp.Domain
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; private set; }
        public Direction Direction { get; private set; }
        public ElevatorState State { get; private set; }
        public int Capacity { get; }
        public List<Passenger> Passengers { get; }

        public Elevator(int id, int capacity, int startFloor = 0)
        {
            Id = id;
            Capacity = capacity;
            CurrentFloor = startFloor;
            Direction = Direction.Idle;
            State = ElevatorState.Stationary;
            Passengers = new List<Passenger>();
        }

        public bool IsFull() => Passengers.Count >= Capacity;

        /// <summary>
        /// Simulates elevator movement floor-by-floor with delay.
        /// Provides real-time feedback to the user in the console.
        /// </summary>
        public void MoveTo(int targetFloor)
        {
            if (targetFloor == CurrentFloor)
            {
                Console.WriteLine($"[Elevator {Id}] Already at floor {CurrentFloor}.");
                return;
            }

            Direction = targetFloor > CurrentFloor ? Direction.Up : Direction.Down;
            State = ElevatorState.Moving;

            Console.WriteLine($"[Elevator {Id}] Starting at floor {CurrentFloor}, moving {Direction} to floor {targetFloor}...");

            // Move step by step
            while (CurrentFloor != targetFloor)
            {
                Thread.Sleep(500); // simulate travel delay
                CurrentFloor += (Direction == Direction.Up) ? 1 : -1;
                Console.WriteLine($"[Elevator {Id}] Now at floor {CurrentFloor}...");
            }

            Stop();
            Console.WriteLine($"[Elevator {Id}] Arrived at floor {CurrentFloor}.");
        }

        public void Stop()
        {
            Direction = Direction.Idle;
            State = ElevatorState.Stationary;
        }

        
        public void LoadPassenger(Passenger passenger)
        {
            if (!IsFull())
            {
                Passengers.Add(passenger);
                State = ElevatorState.Loading;
                Console.WriteLine($"[Elevator {Id}] Passenger added (dest: {passenger.DestinationFloor}).");
            }
            else
            {
                Console.WriteLine($"[Elevator {Id}] Cannot load passenger. Capacity reached! Request must wait.");
            }
        }


        public void UnloadPassengersAtCurrentFloor()
        {
            var offloading = Passengers.RemoveAll(p => p.DestinationFloor == CurrentFloor);
            if (offloading > 0)
            {
                Console.WriteLine($"[Elevator {Id}] {offloading} passenger(s) exited at floor {CurrentFloor}.");
                State = ElevatorState.Unloading;
            }
        }
    }
}

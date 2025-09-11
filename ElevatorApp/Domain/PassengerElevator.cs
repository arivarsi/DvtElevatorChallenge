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
         public List<int> Requests { get; private set; } = new List<int>();
        public PassengerElevator(int id, int capacity, int startFloor = 0)
            : base(id, capacity, startFloor) { }

        // Method to add a request/floor
        public void AddRequest(int startfloor, int targetFloor, int passengerCount = 1, Func<int> idGenerator = null)
        {
            Requests.Add(startfloor);

            int spaceLeft = Capacity - Passengers.Count;
            int toAdd = Math.Min(spaceLeft, passengerCount);

            for (int i = 0; i < toAdd; i++)
            {
                int id = idGenerator?.Invoke() ?? i; // use controller’s ID generator if provided
                LoadPassenger(new Passenger(id, startfloor, targetFloor));
            }
        }

        public override void MoveTo(int startfloor, int targetFloor)
        {
            if (startfloor == CurrentFloor)
            {
                Console.WriteLine($"[PassengerElevator {Id}] Already at pickup floor {CurrentFloor}. Opening doors...");

                // Load passengers waiting at this floor

                // And move directly to target
                if (targetFloor != CurrentFloor)
                {
                    MoveOneStepLoop(startfloor,targetFloor);
                }

                Console.WriteLine($"[PassengerElevator {Id}] Arrived at destination floor {CurrentFloor}.");
                UnloadPassengersAtCurrentFloor();
                Stop();
                return;
            }
            else if (targetFloor == CurrentFloor)
            {
                Console.WriteLine($"[PassengerElevator {Id}] Already at floor {CurrentFloor}.");
                UnloadPassengersAtCurrentFloor();
                return;
            }

            Console.WriteLine($"[PassengerElevator {Id}] Starting at {CurrentFloor}, moving to {startfloor} then moving to {targetFloor}...");
            MoveOneStepLoop(startfloor, targetFloor); Console.WriteLine($"[PassengerElevator {Id}] Arrived at destination floor {CurrentFloor}.");
            UnloadPassengersAtCurrentFloor();
            Stop();
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

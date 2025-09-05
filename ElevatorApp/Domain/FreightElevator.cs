using System;
using ElevatorApp.Domain.Enums;

namespace ElevatorApp.Domain
{
    /// <summary>
    /// Freight elevator: slower movement, higher capacity, 
    /// ignores passenger count (for demo purposes).
    /// </summary>
    public class FreightElevator : ElevatorBase
    {

        public List<int> Requests { get; } = new List<int>();

        public FreightElevator(int id, int capacity, int startFloor = 0)
            : base(id, capacity, startFloor) { }

        /// <summary>Add a freight/utility request for a floor.</summary>
        public void AddFreightRequest(int floor, int loadUnits = 0)
        {
            Requests.Add(floor);
            Console.WriteLine($"[FreightElevator {Id}] Request added for floor {floor} (LoadUnits: {loadUnits}).");
        }

        public override void MoveTo(int floorfrom, int targetFloor)
        {
            Direction = targetFloor > CurrentFloor ? Direction.Up : Direction.Down;
            State = ElevatorState.Moving;

            Console.WriteLine($"[FreightElevator {Id}] Moving goods from floor {CurrentFloor} to {targetFloor}...");
            MoveOneStepLoop(floorfrom,targetFloor);
        }

        public override void Stop()
        {
            Direction = Direction.Idle;
            State = ElevatorState.Stationary;
        }
        

        public override void LoadPassenger(Passenger passenger)
        {
            // Freight elevators can carry "any load" up to capacity
            if (!IsFull())
            {
                Passengers.Add(passenger);
                Console.WriteLine($"[FreightElevator {Id}] Load added (dest: {passenger.DestinationFloor}).");
            }
        }

        public override void UnloadPassengersAtCurrentFloor()
        {
            Passengers.RemoveAll(p => p.DestinationFloor == CurrentFloor);
        }

       
    }
}
using System.Collections.Generic;

namespace ElevatorApp.Domain
{
    public class Floor
    {
        public int FloorNumber { get; }
        public Queue<Passenger> WaitingPassengers { get; }

        public Floor(int floorNumber)
        {
            FloorNumber = floorNumber;
            WaitingPassengers = new Queue<Passenger>();
        }

        public void AddPassenger(Passenger passenger)
        {
            WaitingPassengers.Enqueue(passenger);
        }

        public Passenger? GetNextPassenger()
        {
            return WaitingPassengers.Count > 0 ? WaitingPassengers.Dequeue() : null;
        }
    }
}

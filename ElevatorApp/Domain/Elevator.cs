using System.Collections.Generic;
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

        public void MoveTo(int floor)
        {
            if (floor > CurrentFloor)
                Direction = Direction.Up;
            else if (floor < CurrentFloor)
                Direction = Direction.Down;
            else
                Direction = Direction.Idle;

            CurrentFloor = floor;
            State = ElevatorState.Moving;
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
            }
        }

        public void UnloadPassengersAtCurrentFloor()
        {
            Passengers.RemoveAll(p => p.DestinationFloor == CurrentFloor);
            State = ElevatorState.Unloading;
        }
    }
}

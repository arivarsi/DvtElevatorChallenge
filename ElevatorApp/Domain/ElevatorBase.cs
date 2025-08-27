using System.Collections.Generic;
using ElevatorApp.Domain.Enums;

namespace ElevatorApp.Domain
{
    /// <summary>
    /// Abstract base for all elevator types.
    /// Defines shared behaviour & enforces common contract.
    /// New elevator types (freight, high-speed, glass) should extend this.
    /// </summary>
    public abstract class ElevatorBase
    {
        public int Id { get; }
        public int CurrentFloor { get; protected set; }
        public Direction Direction { get; protected set; }
        public ElevatorState State { get; protected set; }
        public int Capacity { get; }
        public List<Passenger> Passengers { get; }

        protected ElevatorBase(int id, int capacity, int startFloor = 0)
        {
            Id = id;
            Capacity = capacity;
            CurrentFloor = startFloor;
            Direction = Direction.Idle;
            State = ElevatorState.Stationary;
            Passengers = new List<Passenger>();
        }

        public int AvailableCapacity => Capacity - Passengers.Count;

        public bool IsFull() => Passengers.Count >= Capacity;

        // --- Abstract operations (must be implemented differently per type) ---
        public abstract void MoveTo(int targetFloor);
        public abstract void Stop();
        public abstract void LoadPassenger(Passenger passenger);
        public abstract void UnloadPassengersAtCurrentFloor();
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using ElevatorApp.Domain.Enums;

namespace ElevatorApp.Domain
{
    /// <summary>
    /// Abstract base for all elevator types.
    /// Adds real-time, step-wise movement support (per-floor timing), plus events.
    /// Derived classes implement UI/logging or custom rules but should call MoveOneStepLoop(...).
    /// </summary>
    public abstract class ElevatorBase
    {
        public int Id { get; }
        public int CurrentFloor { get; protected set; }
        public Direction Direction { get; protected set; }
        public ElevatorState State { get; protected set; }
        public int Capacity { get; }
        public List<Passenger> Passengers { get; }

        /// <summary>
        /// Seconds to travel between adjacent floors. Default 20s for realism.
        /// Tests can override this static to speed up (e.g., set to 0 or 1).
        /// </summary>
        public static int SecondsPerFloor { get; set; } = 4;

        // --- Events for UI / controller hooks ---
        public event Action<ElevatorBase, int, int>? FloorStep; // (elevator, fromFloor, toFloor)
        public event Action<ElevatorBase, int>? ArrivedAtFloor; // (elevator, floor)
        public event Action<ElevatorBase>? BecameIdle;
        public event Action<ElevatorBase, Direction>? DirectionChanged;

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

        /// <summary>
        /// Derived implementations should call this helper to perform step-wise movement.
        /// This method is synchronous (uses Thread.Sleep) to preserve console-app simplicity
        /// and existing synchronous controller signatures. Tests can accelerate by setting
        /// SecondsPerFloor = 0.
        /// </summary>
        /// <param name="targetFloor">Destination floor</param>
        protected void MoveOneStepLoop(int targetFloor)
        {
            if (targetFloor == CurrentFloor)
            {
                // Already there
                State = ElevatorState.Stationary;
                Direction = Direction.Idle;
                ArrivedAtFloor?.Invoke(this, CurrentFloor);
                BecameIdle?.Invoke(this);
                return;
            }

            Direction = targetFloor > CurrentFloor ? Direction.Up : Direction.Down;
            DirectionChanged?.Invoke(this, Direction);
            State = ElevatorState.Moving;

            while (CurrentFloor != targetFloor)
            {
                int from = CurrentFloor;
                int step = Direction == Direction.Up ? 1 : -1;
                int to = from + step;

                // Simulate travel
                if (SecondsPerFloor > 0)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(SecondsPerFloor));
                }

                CurrentFloor = to;
                FloorStep?.Invoke(this, from, to);
                ArrivedAtFloor?.Invoke(this, CurrentFloor);

                // If we've overshot due to external change, break defensively
                if ((Direction == Direction.Up && CurrentFloor > targetFloor) ||
                    (Direction == Direction.Down && CurrentFloor < targetFloor))
                {
                    break;
                }
            }

            // Stop at target
            Stop();
            BecameIdle?.Invoke(this);
        }

        // --- Abstract operations (kept for compatibility with your design) ---
        public abstract void MoveTo(int targetFloor);
        public abstract void Stop();
        public abstract void LoadPassenger(Passenger passenger);
        public abstract void UnloadPassengersAtCurrentFloor();
    }
}

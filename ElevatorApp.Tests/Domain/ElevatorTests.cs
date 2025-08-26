using ElevatorApp.Domain;
using ElevatorApp.Domain.Enums;
using Xunit;

namespace ElevatorApp.Tests.Domain
{
    public class ElevatorTests
    {
        [Fact]
        public void Elevator_Should_Start_Stationary_And_Empty()
        {
            var elevator = new Elevator(id: 1, capacity: 5);

            Assert.Equal(0, elevator.CurrentFloor);
            Assert.Equal(Direction.Idle, elevator.Direction);
            Assert.Equal(ElevatorState.Stationary, elevator.State);
            Assert.Empty(elevator.Passengers);
        }

        [Fact]
        public void Elevator_Should_Move_And_Update_Floor()
        {
            var elevator = new Elevator(id: 1, capacity: 5);
            elevator.MoveTo(5);

            // After move is complete
            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Equal(Direction.Idle, elevator.Direction); // ✅ final state should be Idle
            Assert.Equal(ElevatorState.Stationary, elevator.State);
        }


        [Fact]
        public void Elevator_Should_Load_And_Unload_Passengers()
        {
            var elevator = new Elevator(id: 1, capacity: 2);
            var passenger = new Passenger(id: 101, destinationFloor: 3);

            elevator.LoadPassenger(passenger);

            Assert.Single(elevator.Passengers);

            elevator.MoveTo(3);
            elevator.UnloadPassengersAtCurrentFloor();

            Assert.Empty(elevator.Passengers);
        }
    }
}

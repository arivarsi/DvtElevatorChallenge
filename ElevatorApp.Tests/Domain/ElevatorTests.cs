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
            ElevatorBase.SecondsPerFloor = 0;
            var elevator = new PassengerElevator(id: 1, capacity: 5, startFloor: 0);

            Assert.Equal(0, elevator.CurrentFloor);
            Assert.Equal(Direction.Idle, elevator.Direction);
            Assert.Equal(ElevatorState.Stationary, elevator.State);
            Assert.Empty(elevator.Passengers);
        }

        [Fact]
        public void Elevator_Should_Move_And_Update_Floor()
        {
            ElevatorBase.SecondsPerFloor = 0;
            var elevator = new PassengerElevator(id: 1, capacity: 5, startFloor: 1);
            elevator.MoveTo(5,0);

            // After move is complete
            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Equal(Direction.Idle, elevator.Direction); // ✅ final state should be Idle
            Assert.Equal(ElevatorState.Stationary, elevator.State);
        }


        [Fact]
        public void Elevator_Should_Load_And_Unload_Passengers()
        {
            ElevatorBase.SecondsPerFloor = 0;
            var elevator = new PassengerElevator(id: 1, capacity: 2, startFloor: 0);
            var passenger = new Passenger(id: 101,currentfloor:2 ,destinationFloor: 3);

            elevator.LoadPassenger(passenger);

            Assert.Single(elevator.Passengers);

            elevator.MoveTo(3,5);
            elevator.UnloadPassengersAtCurrentFloor();

            Assert.Empty(elevator.Passengers);
        }
    }
}

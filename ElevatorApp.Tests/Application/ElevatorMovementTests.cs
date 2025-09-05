using ElevatorApp.Domain;
using ElevatorApp.Domain.Enums;
using Xunit;

namespace ElevatorApp.Tests.Domain
{
    public class ElevatorMovementTests
    {
        [Fact]
        public void Elevator_Should_Reach_Target_Floor()
        {
            ElevatorBase.SecondsPerFloor = 0;
            var elevator = new PassengerElevator(id: 1, capacity: 5, startFloor: 0);

            elevator.MoveTo(3,2);

            Assert.Equal(3, elevator.CurrentFloor);
            Assert.Equal(ElevatorState.Stationary, elevator.State);
        }

        [Fact]
        public void Elevator_Should_Not_Move_If_Already_At_Target()
        {
            ElevatorBase.SecondsPerFloor = 0;
            var elevator = new PassengerElevator(id: 1, capacity: 5, startFloor: 2);

            elevator.MoveTo(2, 5);

            Assert.Equal(2, elevator.CurrentFloor);
            Assert.Equal(ElevatorState.Stationary, elevator.State);
        }
        
        
    }
}
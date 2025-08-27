using System.Collections.Generic;
using ElevatorApp.Domain;
using ElevatorApp.Application;
using Xunit;

namespace ElevatorApp.Tests.Application
{
    public class ElevatorControllerTests
    {
        [Fact]
        public void Should_Dispatch_Nearest_Available_Elevator()
        {
            var elevators = new List<ElevatorBase>
            {
                new PassengerElevator(id: 1, capacity: 5, startFloor: 0),
                new PassengerElevator(id: 2, capacity: 5, startFloor: 10)
            };

            var controller = new ElevatorController(elevators);

            controller.RequestElevator(floor: 2, passengerCount: 2);

            Assert.Equal(2, elevators[0].CurrentFloor); // Elevator 1 should move to floor 2
        }

        [Fact]
        public void Should_Queue_Request_When_All_Elevators_Busy()
        {
            var elevator = new PassengerElevator(id: 1, capacity: 1, startFloor: 0);
            elevator.LoadPassenger(new Passenger(1, 5)); // fill elevator

            var controller = new ElevatorController(new List<ElevatorBase> { elevator });

            controller.RequestElevator(floor: 3, passengerCount: 2);

            // Elevator cannot serve, request should remain queued
            controller.ProcessPendingRequests();

            Assert.NotEqual(3, elevator.CurrentFloor); // Elevator did not move
        }
    }
}
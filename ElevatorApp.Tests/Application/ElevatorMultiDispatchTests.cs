using System.Collections.Generic;
using ElevatorApp.Application;
using ElevatorApp.Domain;
using Xunit;

namespace ElevatorApp.Tests.Application
{
    public class ElevatorMultiDispatchTests
    {
        [Fact]
        public void Test_Dispatch_MultipleElevators()
        {
            ElevatorBase.SecondsPerFloor = 0;
            var elevators = new List<ElevatorBase>
                {
                    new PassengerElevator(id: 1, capacity: 4, startFloor: 0),
                    new PassengerElevator(id: 2, capacity: 4, startFloor: 5),
                };

            var controller = new ElevatorController(elevators);

            // Request: 2 passengers at floor 0 going to floor 3
            controller.RequestElevator(new ElevatorRequest(floorNumber: 0, floortoNumber: 3, passengerCount: 2));

            // Assertions
            var passengerElevator = (PassengerElevator)elevators[0];
            Assert.Single(passengerElevator.Requests);         // nearest elevator got the request
            Assert.Equal(2, elevators[0].Passengers.Count);  // passengers boarded
            var passengerElevator2 = (PassengerElevator)elevators[1];
            Assert.Empty(passengerElevator2.Requests);             // second elevator idle
            Assert.Empty(passengerElevator2.Passengers);
        }


        [Fact]
        public void Should_Split_Large_Group_Across_Nearest_Elevators()
        {
            // Keep distances small so the test remains quick
            ElevatorBase.SecondsPerFloor = 0;
            ElevatorBase.SecondsPerFloor = 0;
            var elevators = new List<ElevatorBase>
            {
                new PassengerElevator(id: 1, capacity: 2, startFloor: 0),
                new PassengerElevator(id: 2, capacity: 2, startFloor: 2)
            };

            var controller = new ElevatorController(elevators);

            // 3 passengers at floor 1; first car can take 2, second takes 1
            controller.RequestElevator(new ElevatorRequest(floorNumber: 1,floortoNumber:3, passengerCount: 3));

            Assert.Equal(1, elevators[0].CurrentFloor);
            Assert.Equal(1, elevators[1].CurrentFloor);
            Assert.Equal(2, elevators[0].Passengers.Count);
            Assert.Equal(1, elevators[1].Passengers.Count);
            Assert.Equal(0, controller.PendingPassengers);
        }
    }
}
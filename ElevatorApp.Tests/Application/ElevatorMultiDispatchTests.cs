using System.Collections.Generic;
using ElevatorApp.Application;
using ElevatorApp.Domain;
using Xunit;

namespace ElevatorApp.Tests.Application
{
    public class ElevatorMultiDispatchTests
    {
        [Fact]
        public void Should_Split_Large_Group_Across_Nearest_Elevators()
        {
            // Keep distances small so the test remains quick
            var elevators = new List<ElevatorBase>
            {
                new PassengerElevator(id: 1, capacity: 2, startFloor: 0),
                new PassengerElevator(id: 2, capacity: 2, startFloor: 2)
            };

            var controller = new ElevatorController(elevators);

            // 3 passengers at floor 1; first car can take 2, second takes 1
            controller.RequestElevator(floor: 1, passengerCount: 3);

            Assert.Equal(1, elevators[0].CurrentFloor);
            Assert.Equal(1, elevators[1].CurrentFloor);
            Assert.Equal(2, elevators[0].Passengers.Count);
            Assert.Equal(1, elevators[1].Passengers.Count);
            Assert.Equal(0, controller.PendingPassengers);
        }
    }
}
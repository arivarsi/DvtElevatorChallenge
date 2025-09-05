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
            ElevatorBase.SecondsPerFloor = 0;
            var elevators = new List<PassengerElevator>
      {
        new PassengerElevator(1, 0),
        new PassengerElevator(2, 5),
      };


    // Closest elevator received request
    Assert.Single(elevators[0].Requests);           // one floor requested
    Assert.Equal(2, elevators[0].Passengers.Count); // 2 passengers added
    Assert.Empty(elevators[1].Requests);            // other elevator idle
    Assert.Empty(elevators[1].Passengers);
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
            controller.RequestElevator(floor: 1,floorto:3, passengerCount: 3);

            Assert.Equal(1, elevators[0].CurrentFloor);
            Assert.Equal(1, elevators[1].CurrentFloor);
            Assert.Equal(2, elevators[0].Passengers.Count);
            Assert.Equal(1, elevators[1].Passengers.Count);
            Assert.Equal(0, controller.PendingPassengers);
        }
    }
}
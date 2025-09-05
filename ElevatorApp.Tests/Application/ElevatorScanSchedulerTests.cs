using ElevatorApp.Domain;
using ElevatorApp.Application;
using Xunit;

namespace ElevatorApp.Tests.Application
{
    public class ElevatorScanSchedulerTests
    {
        [Fact]
        public void Should_Prefer_Elevator_Moving_Towards_Request()
        {
            ElevatorBase.SecondsPerFloor = 0;
            var e1 = new PassengerElevator(id: 1, capacity: 5, startFloor: 1);
            var e2 = new PassengerElevator(id: 2, capacity: 5, startFloor: 5);

            // make e1 moving up (simulate by setting direction and current floor)
            // (we can't directly set Direction, so mimic by calling MoveTo asynchronously then queuing another request)
            e1.MoveTo(4); // e1 will have requests list updated in existing code

            var controller = new ElevatorController(new System.Collections.Generic.List<ElevatorBase>{ e1, e2 });

            // Now request floor 3; e1 is moving up from 1->4 and will pass floor 3 so should be chosen
            controller.RequestElevator(3, 1);

            // The RequestElevator currently adds to PassengerElevator.Requests of chosen elevator
            Assert.Contains(3, ((PassengerElevator)e1).Requests);
        }
    }
}

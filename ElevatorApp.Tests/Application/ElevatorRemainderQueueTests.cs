using System.Collections.Generic;
using ElevatorApp.Application;
using ElevatorApp.Domain;
using Xunit;

namespace ElevatorApp.Tests.Application
{
    public class ElevatorRemainderQueueTests
    {
        [Fact]
        public void Should_Queue_Remainder_When_Total_Capacity_Insufficient()
        {
            ElevatorBase.SecondsPerFloor = 0;
            ElevatorBase.SecondsPerFloor = 0;
            var elevators = new List<ElevatorBase>
            {
                new PassengerElevator(id: 1, capacity: 1, startFloor: 0),
                new PassengerElevator(id: 2, capacity: 1, startFloor: 0)
            };

            var controller = new ElevatorController(elevators);

            // Request exceeds total capacity (2)
            controller.RequestElevator(floor: 0, passengerCount: 5);

            // 2 onboard, 3 should be queued
            Assert.Equal(2, elevators[0].Passengers.Count + elevators[1].Passengers.Count);
            Assert.Equal(3, controller.PendingPassengers);
        }
    }
}
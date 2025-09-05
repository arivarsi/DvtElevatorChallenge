using System.Collections.Generic;
using ElevatorApp.Application;
using ElevatorApp.Domain;
using ElevatorApp.Domain.Enums;
using Xunit;

namespace ElevatorApp.Tests.Application
{
    public class ElevatorDirectionAwareTests
    {
        [Fact]
        public void Should_Prefer_Elevator_Moving_Up_When_Request_Is_Up()
        {
            ElevatorBase.SecondsPerFloor = 0;

            var upElevator = new PassengerElevator(1, capacity: 5, startFloor: 1);
            var downElevator = new PassengerElevator(2, capacity: 5, startFloor: 5);

            // simulate state


            var controller = new ElevatorController(new List<ElevatorBase> { upElevator, downElevator });

            // request: pickup at floor 2, going to 8 (direction = Up)
            var chosen = controller.GetType()
                                   .GetMethod("SelectBestElevator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                   .Invoke(controller, new object[] { 2, 8 });

            Assert.Equal(upElevator, chosen);
        }

        [Fact]
        public void Should_Prefer_Idle_Elevator_If_Closer()
        {
            ElevatorBase.SecondsPerFloor = 0;

            var idleNear = new PassengerElevator(1, capacity: 5, startFloor: 3);
            var idleFar = new PassengerElevator(2, capacity: 5, startFloor: 10);

            var controller = new ElevatorController(new List<ElevatorBase> { idleNear, idleFar });

            var chosen = controller.GetType()
                                   .GetMethod("SelectBestElevator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                   .Invoke(controller, new object[] { 4, 7 });

            Assert.Equal(idleNear, chosen);
        }

        [Fact]
        public void Should_Avoid_Elevator_Moving_Opposite_Direction()
        {
            ElevatorBase.SecondsPerFloor = 0;

            var goingDown = new PassengerElevator(1, capacity: 5, startFloor: 6);
            var idle = new PassengerElevator(2, capacity: 5, startFloor: 2);

            var controller = new ElevatorController(new List<ElevatorBase> { goingDown, idle });

            // Request pickup at floor 5, going up to 8
            var chosen = controller.GetType()
                                   .GetMethod("SelectBestElevator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                   .Invoke(controller, new object[] { 5, 8 });

            Assert.Equal(idle, chosen); // idle should be chosen, not opposite-moving one
        }
    }
}



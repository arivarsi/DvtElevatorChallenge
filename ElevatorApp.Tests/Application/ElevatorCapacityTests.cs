using System.Collections.Generic;
using ElevatorApp.Application;
using ElevatorApp.Domain;
using Xunit;

namespace ElevatorApp.Tests.Application
{
    public class ElevatorCapacityTests
    {
        [Fact]
        public void Elevator_Should_Not_Exceed_Capacity()
        {
            ElevatorBase.SecondsPerFloor = 0;
            var elevator = new PassengerElevator(id: 1, capacity: 2);

            elevator.LoadPassenger(new Passenger(1,6, 5));
            elevator.LoadPassenger(new Passenger(2,6, 6));
            elevator.LoadPassenger(new Passenger(3, 8,7)); // should be rejected

            Assert.Equal(2, elevator.Passengers.Count); // still only 2 loaded
        }

        [Fact]
        public void Controller_Should_Queue_Request_If_Elevator_Full()
        {
            ElevatorBase.SecondsPerFloor = 0;
            ElevatorBase.SecondsPerFloor = 0;
            var elevators = new List<ElevatorBase> { new PassengerElevator(id: 1, capacity: 1) };
            var controller = new ElevatorController(elevators);

            // first request fills the elevator
            controller.RequestElevator(new ElevatorRequest(floorNumber: 1,floortoNumber:6, passengerCount: 1));

            // second request should be queued
            controller.RequestElevator(new ElevatorRequest(floorNumber: 2, floortoNumber:6,passengerCount: 2));

            controller.ProcessPendingRequests();

            // elevator cannot take more than 1 passenger
            Assert.True(elevators[0].IsFull());
        }
    }
}
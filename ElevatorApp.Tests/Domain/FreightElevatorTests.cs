using ElevatorApp.Domain;
using Xunit;

namespace ElevatorApp.Tests.Domain
{
    public class FreightElevatorTests
    {
        [Fact]
        public void FreightElevator_Should_Move_To_Target_Floor()
        {
            var freight = new FreightElevator(id: 99, capacity: 10, startFloor: 0);
            freight.MoveTo(5);

            Assert.Equal(5, freight.CurrentFloor);
            Assert.Equal(0, freight.Passengers.Count); // none by default
        }

        [Fact]
        public void FreightElevator_Should_Load_Items()
        {
            var freight = new FreightElevator(id: 99, capacity: 2, startFloor: 0);
            freight.LoadPassenger(new Passenger(1, 2));
            freight.LoadPassenger(new Passenger(2, 3));

            Assert.Equal(2, freight.Passengers.Count);
        }
    }
}
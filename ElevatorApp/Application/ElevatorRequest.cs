namespace ElevatorApp.Application
{
    /// <summary>
    /// Represents a request for an elevator at a specific floor with passengers waiting.
    /// </summary>
    public class ElevatorRequest
    {
        public int FloorNumber { get; }
        public int PassengerCount { get; }
        public int FloortoNumber { get; set; }

        public ElevatorRequest(int floorNumber, int floortoNumber, int passengerCount)
        {
            FloorNumber = floorNumber;
            PassengerCount = passengerCount;
            FloortoNumber = floortoNumber;
        }
    }
}
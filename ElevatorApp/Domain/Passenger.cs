namespace ElevatorApp.Domain
{
    public class Passenger
    {
        public int Id { get; }
        public int CurrentFloor { get; set; }
        public int DestinationFloor { get; }

        public Passenger(int id, int currentfloor,int destinationFloor)
        {
            Id = id;
            CurrentFloor = currentfloor;
            DestinationFloor = destinationFloor;
        }
    }
}

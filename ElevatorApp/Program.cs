using ElevatorApp.Domain;
using ElevatorApp.Application;
using ElevatorApp.UI;

Console.WriteLine("Initializing Elevator System...");

// Create elevators (for now fixed setup: 3 elevators, capacity 5 each)
var elevators = new List<ElevatorBase>
{
    new PassengerElevator(id: 1, capacity: 12),
    new PassengerElevator(id: 2, capacity: 12),
    new FreightElevator(id: 3, capacity: 20) // new type, works seamles10sly
};

var controller = new ElevatorController(elevators);
var ui = new ConsoleUI(controller);
ui.Run();

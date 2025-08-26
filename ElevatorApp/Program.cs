using ElevatorApp.Domain;
using ElevatorApp.Application;
using ElevatorApp.UI;

Console.WriteLine("Initializing Elevator System...");

// Create elevators (for now fixed setup: 3 elevators, capacity 5 each)
var elevators = new List<Elevator>
{
    new Elevator(id: 1, capacity: 5),
    new Elevator(id: 2, capacity: 5),
    new Elevator(id: 3, capacity: 5)
};

var controller = new ElevatorController(elevators);
var ui = new ConsoleUI(controller);

// Run interactive console loop
ui.Run();
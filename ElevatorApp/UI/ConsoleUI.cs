using System;
using System.Collections.Generic;
using ElevatorApp.Application;
using ElevatorApp.Domain;

namespace ElevatorApp.UI
{
    /// <summary>
    /// Console-based user interface for interacting with the elevator system.
    /// Provides a simple menu loop to simulate real-time elevator usage.
    /// </summary>
    public class ConsoleUI
    {
        private readonly ElevatorController _controller;

        public ConsoleUI(ElevatorController controller)
        {
            _controller = controller;
        }

        /// <summary>
        /// Starts the UI loop.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== DVT Elevator Challenge ===");
                Console.WriteLine("1. Call Elevator");
                Console.WriteLine("2. Process Pending Requests");
                Console.WriteLine("3. Show Elevator Status");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleElevatorRequest();
                        break;
                    case "2":
                        _controller.ProcessPendingRequests();
                        Console.WriteLine("Processed pending requests.");
                        Pause();
                        break;
                    case "3":
                        _controller.PrintElevatorStatus();
                        Pause();
                        break;
                    case "4":
                        Console.WriteLine("Exiting... Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Pause();
                        break;
                }
            }
        }

        /// <summary>
        /// Prompts user for floor and passenger input, then requests an elevator.
        /// </summary>
        private void HandleElevatorRequest()
        {
            Console.Write("Enter floor number: ");
            if (!int.TryParse(Console.ReadLine(), out var floor))
            {
                Console.WriteLine("Invalid floor number.");
                Pause();
                return;
            }

            Console.Write("Enter number of passengers: ");
            if (!int.TryParse(Console.ReadLine(), out var passengers))
            {
                Console.WriteLine("Invalid passenger count.");
                Pause();
                return;
            }

            _controller.RequestElevator(floor, passengers);
            _controller.PrintElevatorStatus();
            Pause();
        }

        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}

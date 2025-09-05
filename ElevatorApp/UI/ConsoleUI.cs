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
            // Start a background refresh loop
            var stop = false;
            Task.Run(() =>
            {
                while (!stop)
                {
                    Console.Clear();
                    Console.WriteLine("=== DVT Elevator Challenge (Real-time) ===");
                    _controller.PrintElevatorStatus();
                    Console.WriteLine("Controls: 1) Call Elevator  2) Process Pending Requests  3) Show Elevator Status  q) Quit");

                    System.Threading.Thread.Sleep(1000); // refresh every second
                }
            });


            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == 'q') break;

                    switch (key.KeyChar)
                    {
                        
                        case '1':
                            //stop printing elevator status  because we do not want printing overwriting instructions for data entry
                            stop = true;
                            Console.Write(" Enter floor to pick up: ");
                            if (int.TryParse(Console.ReadLine(), out var floor))
                            {
                                Console.Write(" Enter destination Floor: ");
                                int.TryParse(Console.ReadLine(), out var floorto);

                                Console.Write(" Passengers: ");
                                int.TryParse(Console.ReadLine(), out var pcount);

                                _controller.RequestElevator(floor,floorto, pcount == 0 ? 1 : pcount);
                            }

                            stop = false; //continue after data entry
                            break;
                        case '2':
                            _controller.ProcessPendingRequests();
                            break;
                        case '3':
                            Console.Clear();
                            _controller.PrintElevatorStatus();
                            Console.WriteLine("Press any key to continue..."); Console.ReadKey(true);
                            break;
                        default:
                            break;
                    }
                   // stop = false; //continue after data entry
                }
                else
                {
                    System.Threading.Thread.Sleep(200);
                }
            }

           stop = false;//continue after data entry
        }

        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}

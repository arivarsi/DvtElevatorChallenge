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
                    //Console.Clear();
                    Console.WriteLine("=== DVT Elevator Challenge (Real-time) ===");
                    _controller.PrintElevatorStatus();
                    Console.WriteLine("Controls: 1) Call Elevator  2) Process Pending Requests  3) Show Elevator Status  q) Quit");

                    System.Threading.Thread.Sleep(1000); // refresh every second
                }
            });

            while (true)
            {
                //read for any key entry but also read for quiting the program
                var key = Console.ReadKey(true);

                if (key.KeyChar == 'q') break;

                switch (key.KeyChar)
                {
                    case '1':
                        stop = true;
                        Console.Write(" Enter floor to pick up: ");
                        int floor = int.Parse(Console.ReadLine());

                        Console.Write(" Enter destination Floor: ");
                        int floorto = int.Parse(Console.ReadLine());

                        Console.Write(" Passengers: ");
                        int pcount = int.Parse(Console.ReadLine());

                        _controller.RequestElevator(new ElevatorRequest(floor, floorto, pcount == 0 ? 1 : pcount));
                        _controller.ProcessPendingRequests();
                        _controller.PrintElevatorStatus();
                        stop = false;
                        break;

                    case '2':
                        _controller.ProcessPendingRequests();
                        _controller.PrintElevatorStatus();
                        break;

                    case '3':
                        _controller.PrintElevatorStatus();
                        break;
                }
            }

            stop = false;
        }

        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}

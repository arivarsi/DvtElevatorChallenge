# DVT Elevator Challenge 🚀

## Project Overview
This project is a console-based elevator simulation implemented in **C#** for the DVT Elevator Challenge.

It models both **passenger** and **freight elevators**, supports multiple floors, and provides **real-time simulation** of elevator movement and status updates. The application follows **OOP and SOLID principles** and is structured for extensibility and testing.

---

## Features

1. **Real-Time Elevator Status**
   - Floor-by-floor movement with configurable timing (`SecondsPerFloor`).
   - Direction, state, and passenger/load count displayed continuously in the console.

2. **Interactive Elevator Control**
   - Users call elevators via the console, specifying floor and passenger or load count.
   - Commands are interactive through the `ConsoleUI`.

3. **Multiple Floors and Elevators**
   - System supports any number of elevators (passenger or freight).
   - Central controller coordinates elevator assignments.

4. **Efficient Elevator Dispatching**
   - Implements a **SCAN-like disk scheduling algorithm** for realistic dispatch decisions.
   - Considers direction, distance, and load when choosing an elevator.

5. **Passenger Limit Handling**
   - Enforces capacity per elevator.
   - Excess passengers are deferred as pending requests.

6. **Different Elevator Types**
   - **PassengerElevator**: normal transport with passenger requests.
   - **FreightElevator**: handles goods/load requests separately.
   - Architecture allows for new types (high-speed, glass, service, etc.).

7. **Real-Time Operation**
   - Movement is simulated floor-by-floor with delays (default 2 seconds per floor).
   - Events (`FloorStep`, `ArrivedAtFloor`) trigger console updates.

---

## Tech Stack
- .NET 8 (LTS)
- xUnit for unit testing
- Clean Architecture principles
- SOLID design

---

## Architecture

- **Domain Layer**
  - `ElevatorBase`: abstract class with core elevator logic.
  - `PassengerElevator` and `FreightElevator`: concrete implementations.
  - `Enums`: `Direction`, `ElevatorState`.
  - `Passenger`: passenger entity.

- **Application Layer**
  - `ElevatorController`: manages requests, scheduling, and dispatching.

- **UI Layer**
  - `ConsoleUI`: interactive console loop with real-time status refresh.

- **Tests**
  - Implemented with **xUnit**.
  - Cover elevator movement, passenger/freight requests, and scheduling logic.

---

## SOLID Principles Alignment

- **SRP**: Clear separation of concerns across domain, controller, and UI.
- **OCP**: New elevator types can be added without modifying existing code.
- **LSP**: All elevator types extend `ElevatorBase` and can be substituted.
- **ISP**: Interfaces can later split control, passenger, and freight operations.
- **DIP**: Controller depends on `ElevatorBase` abstraction, not concrete types.

---

## Unit Tests

The test suite covers:

- Step-by-step elevator movement.
- Passenger request handling (`PassengerElevator`).
- Freight request handling (`FreightElevator`).
- SCAN scheduling logic in controller.

Note: Tests set `ElevatorBase.SecondsPerFloor = 0` to run instantly.

Run tests with:
```bash
dotnet test
```

---

## Running the Project

1. Build:
```bash
dotnet build
```

2. Run:
```bash
dotnet run --project ElevatorApp
```

3. Use console commands:
   - `1` — Call Elevator (floor + passenger/load count).
   - `2` — Process pending requests.
   - `3` — Show elevator status.
   - `q` — Quit simulation.

---

## Future Improvements

- Async `MoveToAsync` implementation (non-blocking movement).
- Advanced scheduling (LOOK, C-LOOK, priority queues).
- Event-driven UI updates instead of polling loop.
- Performance metrics: average wait time, utilisation rate, max load.

---

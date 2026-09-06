# Tic Tac Toe

A clean, full-stack Tic Tac Toe application with an Angular frontend and a .NET Web API backend.

The project is intentionally small and readable: domain rules live in the backend, UI state lives in the Angular app, and both sides are covered by focused unit tests.

## Features

- Two-player mode
- Player vs computer mode
- Computer move priority:
  1. Win if possible
  2. Block X if X can win next
  3. Take the center
  4. Take a corner
  5. Take any available cell
- Move validation
- Win and draw detection
- Winning-cell highlight
- Move history
- Undo last move
  - Two-player mode: undo one move
  - Computer mode: undo the human and computer move together
- Reset current game
- Session scoreboard
- Reset scoreboard
- Premium responsive UI
- Short "Computer is thinking..." delay for better UX

## Tech stack

### Backend

- .NET 10
- ASP.NET Core Minimal API
- xUnit
- In-memory repository

### Frontend

- Angular 22
- Angular signals
- Vitest
- SCSS

## Project structure

```text
backend/
  TicTacToe.Api/
    Dtos/
    Endpoints/
    Models/
    Repositories/
    Services/
  TicTacToe.Tests/

frontend/
  src/app/
    game-board/
    move-history/
    scoreboard-panel/
```

## Run locally

### 1. Start the backend

```bash
cd backend
dotnet run --project TicTacToe.Api
```

The API runs on:

```text
http://localhost:5208
```

### 2. Start the frontend

Open a second terminal:

```bash
cd frontend
npm install
npm start
```

The app runs on:

```text
http://localhost:4200
```

The frontend expects the API at:

```text
http://localhost:5208/api
```

## Test

### Backend tests

```bash
cd backend
dotnet test
```

### Frontend tests

```bash
cd frontend
npm test
```

### Frontend production build

```bash
cd frontend
npm run build
```

## API

All endpoints are under `/api`.

### Create game

```http
POST /api/games
```

Request:

```json
{
  "mode": "TwoPlayer"
}
```

or:

```json
{
  "mode": "VsComputer"
}
```

### Get game

```http
GET /api/games/{id}
```

### Submit move

```http
POST /api/games/{id}/moves
```

Request:

```json
{
  "player": "X",
  "cell": 4
}
```

Cells are zero-based:

```text
0 | 1 | 2
3 | 4 | 5
6 | 7 | 8
```

### Undo move

```http
POST /api/games/{id}/undo
```

### Reset game

```http
POST /api/games/{id}/reset
```

### Get scoreboard

```http
GET /api/scoreboard
```

### Reset scoreboard

```http
POST /api/scoreboard/reset
```

## Response shape

Game endpoints return:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "board": [null, null, null, null, "X", null, null, null, "O"],
  "currentPlayer": "X",
  "mode": "VsComputer",
  "status": "InProgress",
  "winner": null,
  "winningCells": [],
  "history": [
    { "number": 1, "player": "X", "cell": 4 },
    { "number": 2, "player": "O", "cell": 8 }
  ],
  "scoreboard": {
    "xWins": 0,
    "oWins": 0,
    "draws": 0
  }
}
```

Validation errors return:

```json
{
  "error": "Cell is already occupied."
}
```

## Design decisions

- Minimal API instead of controllers because the API surface is small and endpoint functions are already separated in `GameEndpoints`.
- Repository pattern is used only where it helps: hiding in-memory game storage behind `IGameRepository`.
- Application orchestration lives in `GameService`; domain rules stay inside `Game`, `BoardAnalyzer`, `ComputerPlayer`, and `Scoreboard`.
- No database is used because the requirement is session/in-memory gameplay.
- No state pattern is used because the game has only three simple statuses. A state pattern would add ceremony without reducing complexity.
- The computer player is deterministic because the requirement defines a fixed priority order.
- The frontend simulates a small computer-thinking delay for UX, while the backend still returns the complete updated state as the source of truth.

## Clean-code approach

- Small domain methods with named predicates
- Pure board analysis separated from turn orchestration
- Computer strategy isolated from game state mutation
- DTO mapping separated from endpoints
- Focused tests for domain rules, service behavior, and UI behavior
- Dependencies kept minimal; unused OpenAPI, forms, and router packages were removed

## Assumptions

- Game state is stored in memory and is lost when the backend restarts.
- X always starts.
- In computer mode, the human is X and the computer is O.
- Undo is disabled after a game is completed.
- In computer mode, undo removes the latest human/computer pair.
- Scoreboard counts a completed game once; if the game is reset, the replay can be counted again.

## Current limitations

- No persistent database
- No authentication
- No online multiplayer
- No deployment configuration
- No end-to-end test suite

These are intentionally outside the current scope.


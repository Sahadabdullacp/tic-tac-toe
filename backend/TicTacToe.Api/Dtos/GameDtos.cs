using TicTacToe.Api.Models;

namespace TicTacToe.Api.Dtos;

public record CreateGameRequest(GameMode Mode);

public record MoveRequest(Player Player, int Cell);

public record MoveDto(int Number, string Player, int Cell);

public record ScoreboardDto(int XWins, int OWins, int Draws);

public record GameStateDto(
    Guid Id,
    string?[] Board,
    string CurrentPlayer,
    GameMode Mode,
    GameStatus Status,
    string? Winner,
    int[] WinningCells,
    MoveDto[] History,
    ScoreboardDto Scoreboard);

using TicTacToe.Api.Dtos;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

/// <summary>Maps domain models to their wire-facing DTOs.</summary>
public static class GameMapping
{
    public static GameStateDto ToDto(this Game game, Scoreboard scoreboard) => new(
        game.Id,
        game.Board.Select(p => p?.ToString()).ToArray(),
        game.CurrentPlayer.ToString(),
        game.Mode,
        game.Status,
        game.Winner?.ToString(),
        game.WinningCells,
        game.History.Select(m => new MoveDto(m.Number, m.Player.ToString(), m.Cell)).ToArray(),
        scoreboard.ToDto());

    public static ScoreboardDto ToDto(this Scoreboard scoreboard) =>
        new(scoreboard.XWins, scoreboard.OWins, scoreboard.Draws);
}

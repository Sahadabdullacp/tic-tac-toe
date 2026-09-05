using TicTacToe.Api.Models;

namespace TicTacToe.Api.Repositories;

/// <summary>Persists game sessions and the single session-level scoreboard.
/// Swap the registered implementation (e.g. to a SQLite-backed one) without touching callers.</summary>
public interface IGameRepository
{
    Scoreboard Scoreboard { get; }
    Game Add(Game game);
    Game? FindById(Guid id);
}

using System.Collections.Concurrent;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Repositories;

/// <summary>In-memory <see cref="IGameRepository"/> backed by a thread-safe dictionary. Registered as a singleton.</summary>
public class InMemoryGameRepository : IGameRepository
{
    private readonly ConcurrentDictionary<Guid, Game> _games = new();

    public Scoreboard Scoreboard { get; } = new();

    public Game Add(Game game)
    {
        _games[game.Id] = game;
        return game;
    }

    public Game? FindById(Guid id) => _games.GetValueOrDefault(id);
}

using TicTacToe.Api.Models;
using TicTacToe.Api.Repositories;

namespace TicTacToe.Api.Services;

/// <summary>Application service orchestrating game creation, moves, undo and reset against the repository.
/// Endpoints depend on this instead of talking to <see cref="IGameRepository"/> or <see cref="Game"/> directly.</summary>
public class GameService(IGameRepository repository)
{
    public Scoreboard Scoreboard => repository.Scoreboard;

    public Game CreateGame(GameMode mode) => repository.Add(new Game(mode));

    public Game? FindGame(Guid id) => repository.FindById(id);

    public MoveResult SubmitMove(Game game, Player player, int cell)
    {
        var result = game.Move(player, cell);
        if (!result.Success) return result;

        repository.Scoreboard.RecordGameOnce(game);
        PlayComputerTurnIfDue(game);
        return result;
    }

    private void PlayComputerTurnIfDue(Game game)
    {
        if (!IsComputersTurn(game)) return;

        game.Move(Player.O, ComputerPlayer.ChooseMove(game.Board));
        repository.Scoreboard.RecordGameOnce(game);
    }

    private static bool IsComputersTurn(Game game) =>
        game is { Status: GameStatus.InProgress, Mode: GameMode.VsComputer, CurrentPlayer: Player.O };

    public MoveResult Undo(Game game) => game.Undo();

    public void ResetGame(Game game)
    {
        repository.Scoreboard.Forget(game);
        game.Reset();
    }

    public void ResetScoreboard() => repository.Scoreboard.Reset();
}

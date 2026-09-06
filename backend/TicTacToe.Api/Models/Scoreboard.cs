namespace TicTacToe.Api.Models;

/// <summary>Session-level counts of completed games. Update once per finished game.</summary>
public class Scoreboard
{
    private readonly HashSet<Guid> _recordedGameIds = [];

    public int XWins { get; private set; }
    public int OWins { get; private set; }
    public int Draws { get; private set; }

    public void RecordResult(GameStatus status, Player? winner)
    {
        if (IsWin(status)) IncrementWinner(winner);
        else if (IsDraw(status)) Draws++;
    }

    private static bool IsWin(GameStatus status) => status == GameStatus.Won;
    private static bool IsDraw(GameStatus status) => status == GameStatus.Draw;

    private void IncrementWinner(Player? winner)
    {
        if (winner == Player.X) XWins++;
        else if (winner == Player.O) OWins++;
    }

    /// <summary>Records a completed game's result, ignoring repeat calls for the same game.</summary>
    public void RecordGameOnce(Game game)
    {
        if (IsInProgress(game)) return;
        if (AlreadyRecorded(game)) return;

        RecordResult(game.Status, game.Winner);
    }

    public void Forget(Game game) => _recordedGameIds.Remove(game.Id);

    private static bool IsInProgress(Game game) => game.Status == GameStatus.InProgress;
    private bool AlreadyRecorded(Game game) => !_recordedGameIds.Add(game.Id);

    public void Reset()
    {
        XWins = 0;
        OWins = 0;
        Draws = 0;
        _recordedGameIds.Clear();
    }
}

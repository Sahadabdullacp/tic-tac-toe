namespace TicTacToe.Api.Models;

/// <summary>Domain model owning turn orchestration, move validation, undo and history. Delegates win/draw
/// detection to <see cref="BoardAnalyzer"/>.</summary>
public class Game
{
    public Guid Id { get; } = Guid.NewGuid();
    public GameMode Mode { get; }
    public Player?[] Board { get; private set; } = new Player?[9];
    public Player CurrentPlayer { get; private set; } = Player.X;
    public GameStatus Status { get; private set; } = GameStatus.InProgress;
    public Player? Winner { get; private set; }
    public int[] WinningCells { get; private set; } = [];
    public List<Move> History { get; private set; } = [];

    public Game(GameMode mode) => Mode = mode;

    public MoveResult Move(Player player, int cell)
    {
        var error = ValidateMove(player, cell);
        if (error is not null) return new MoveResult(false, error);

        PlaceMark(player, cell);
        return new MoveResult(true);
    }

    private string? ValidateMove(Player player, int cell) => this switch
    {
        _ when IsGameOver() => "Game already completed.",
        _ when IsOutsideBoard(cell) => "Cell is outside the board.",
        _ when IsWrongPlayersTurn(player) => "It is not this player's turn.",
        _ when IsOccupied(cell) => "Cell is already occupied.",
        _ => null,
    };

    private bool IsGameOver() => Status != GameStatus.InProgress;
    private static bool IsOutsideBoard(int cell) => cell is < 0 or > 8;
    private bool IsWrongPlayersTurn(Player player) => player != CurrentPlayer;
    private bool IsOccupied(int cell) => Board[cell] is not null;

    private void PlaceMark(Player player, int cell)
    {
        Board[cell] = player;
        History.Add(new Move(History.Count + 1, player, cell));
        EvaluateGameEnd();
        if (Status == GameStatus.InProgress)
            CurrentPlayer = Opponent(player);
    }

    private static Player Opponent(Player player) => player == Player.X ? Player.O : Player.X;

    public MoveResult Undo()
    {
        if (IsGameOver())
            return new MoveResult(false, "Cannot undo after game completion.");

        var movesToUndo = MovesToUndoForMode();
        if (NotEnoughHistoryToUndo(movesToUndo))
            return new MoveResult(false, "No moves to undo.");

        for (var i = 0; i < movesToUndo; i++)
            RemoveLastMove();

        EvaluateGameEnd();
        CurrentPlayer = NextPlayerAfter(History.Count);
        return new MoveResult(true);
    }

    private int MovesToUndoForMode() => Mode == GameMode.VsComputer ? 2 : 1;
    private bool NotEnoughHistoryToUndo(int movesToUndo) => History.Count < movesToUndo;

    private static Player NextPlayerAfter(int movesPlayed) => movesPlayed % 2 == 0 ? Player.X : Player.O;

    public void Reset()
    {
        Board = new Player?[9];
        CurrentPlayer = Player.X;
        Status = GameStatus.InProgress;
        Winner = null;
        WinningCells = [];
        History = [];
    }

    private void RemoveLastMove()
    {
        var last = History[^1];
        Board[last.Cell] = null;
        History.RemoveAt(History.Count - 1);
    }

    private void EvaluateGameEnd()
    {
        var winningLine = BoardAnalyzer.FindWinningLine(Board);
        if (winningLine is not null)
        {
            Status = GameStatus.Won;
            Winner = winningLine.Winner;
            WinningCells = winningLine.Cells;
        }
        else if (BoardAnalyzer.IsBoardFull(Board))
        {
            Status = GameStatus.Draw;
        }
    }
}

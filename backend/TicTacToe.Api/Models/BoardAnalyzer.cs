namespace TicTacToe.Api.Models;

public record WinningLine(Player Winner, int[] Cells);

/// <summary>Pure board analysis: is there a winning line, or is the board full? Knows nothing about turns or history.</summary>
public static class BoardAnalyzer
{
    private static readonly int[][] Lines =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8], // rows
        [0, 3, 6], [1, 4, 7], [2, 5, 8], // columns
        [0, 4, 8], [2, 4, 6],            // diagonals
    ];

    public static WinningLine? FindWinningLine(Player?[] board)
    {
        var line = Lines.FirstOrDefault(l => IsCompletedLine(board, l));
        return line is null ? null : new WinningLine(board[line[0]]!.Value, line);
    }

    public static bool IsBoardFull(Player?[] board) => board.All(cell => cell is not null);

    private static bool IsCompletedLine(Player?[] board, int[] line) =>
        board[line[0]] is not null && LineIsAllSamePlayer(board, line);

    private static bool LineIsAllSamePlayer(Player?[] board, int[] line) =>
        line.Select(cell => board[cell]).Distinct().Count() == 1;
}

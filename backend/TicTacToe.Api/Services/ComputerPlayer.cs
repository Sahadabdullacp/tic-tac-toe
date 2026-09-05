using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

/// <summary>Chooses O's move: win > block > center > corner > any available cell.</summary>
public static class ComputerPlayer
{
    private static readonly int[][] Lines =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8],
        [0, 3, 6], [1, 4, 7], [2, 5, 8],
        [0, 4, 8], [2, 4, 6],
    ];
    private static readonly int[] Corners = [0, 2, 6, 8];
    private const int Center = 4;

    public static int ChooseMove(Player?[] board) =>
        FindWinningMove(board, Player.O)
        ?? FindWinningMove(board, Player.X)
        ?? Empty(board, Center)
        ?? FindEmptyCorner(board)
        ?? FindAnyEmptyCell(board);

    private static int? FindEmptyCorner(Player?[] board) =>
        Corners.Select(cell => Empty(board, cell)).FirstOrDefault(cell => cell is not null);

    private static int FindAnyEmptyCell(Player?[] board) =>
        Enumerable.Range(0, 9).First(cell => IsEmpty(board, cell));

    private static int? FindWinningMove(Player?[] board, Player player) =>
        Lines
            .Where(line => CanCompleteLine(board, line, player))
            .Select(line => EmptyCellIn(board, line))
            .Cast<int?>()
            .FirstOrDefault();

    private static bool CanCompleteLine(Player?[] board, int[] line, Player player) =>
        HasExactlyTwo(board, line, player) && HasExactlyOneEmptyCell(board, line);

    private static bool HasExactlyTwo(Player?[] board, int[] line, Player player) =>
        line.Count(cell => board[cell] == player) == 2;

    private static bool HasExactlyOneEmptyCell(Player?[] board, int[] line) =>
        line.Count(cell => IsEmpty(board, cell)) == 1;

    private static int EmptyCellIn(Player?[] board, int[] line) =>
        line.First(cell => IsEmpty(board, cell));

    private static bool IsEmpty(Player?[] board, int cell) => board[cell] is null;

    private static int? Empty(Player?[] board, int cell) => IsEmpty(board, cell) ? cell : null;
}

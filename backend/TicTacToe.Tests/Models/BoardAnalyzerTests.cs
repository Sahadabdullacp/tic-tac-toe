using TicTacToe.Api.Models;
using Xunit;

namespace TicTacToe.Tests.Models;

public class BoardAnalyzerTests
{
    [Fact]
    public void FindWinningLine_ReturnsNull_WhenNoLineComplete()
    {
        Player?[] board = [Player.X, Player.O, null, null, null, null, null, null, null];

        Assert.Null(BoardAnalyzer.FindWinningLine(board));
    }

    [Fact]
    public void FindWinningLine_ReturnsWinnerAndCells_WhenRowComplete()
    {
        Player?[] board = [Player.X, Player.X, Player.X, null, null, null, null, null, null];

        var result = BoardAnalyzer.FindWinningLine(board);

        Assert.NotNull(result);
        Assert.Equal(Player.X, result.Winner);
        Assert.Equal(new[] { 0, 1, 2 }, result.Cells);
    }

    [Fact]
    public void IsBoardFull_ReturnsFalse_WhenEmptyCellsRemain()
    {
        Player?[] board = new Player?[9];

        Assert.False(BoardAnalyzer.IsBoardFull(board));
    }

    [Fact]
    public void IsBoardFull_ReturnsTrue_WhenNoEmptyCellsRemain()
    {
        Player?[] board =
        [
            Player.X, Player.O, Player.X,
            Player.X, Player.O, Player.O,
            Player.O, Player.X, Player.X,
        ];

        Assert.True(BoardAnalyzer.IsBoardFull(board));
    }
}

using TicTacToe.Api.Models;
using TicTacToe.Api.Services;
using Xunit;

namespace TicTacToe.Tests;

public class ComputerPlayerTests
{
    [Fact]
    public void PicksWinningMove_WhenAvailable()
    {
        Player?[] board = [Player.O, Player.O, null, Player.X, Player.X, null, null, null, null];

        var move = ComputerPlayer.ChooseMove(board);

        Assert.Equal(2, move);
    }

    [Fact]
    public void BlocksOpponent_WhenNoWinAvailable()
    {
        Player?[] board = [Player.X, Player.X, null, null, Player.O, null, null, null, null];

        var move = ComputerPlayer.ChooseMove(board);

        Assert.Equal(2, move);
    }

    [Fact]
    public void TakesCenter_WhenAvailableAndNoWinOrBlock()
    {
        Player?[] board = [Player.X, null, null, null, null, null, null, null, null];

        var move = ComputerPlayer.ChooseMove(board);

        Assert.Equal(4, move);
    }

    [Fact]
    public void TakesCorner_WhenCenterTaken()
    {
        Player?[] board = [null, null, null, null, Player.X, null, null, null, null];

        var move = ComputerPlayer.ChooseMove(board);

        Assert.Contains(move, new[] { 0, 2, 6, 8 });
    }

    [Fact]
    public void TakesAnyAvailableCell_WhenNoOtherPriorityMatches()
    {
        Player?[] board = [Player.X, Player.O, Player.X, Player.O, Player.X, Player.O, Player.O, Player.X, null];

        var move = ComputerPlayer.ChooseMove(board);

        Assert.Equal(8, move);
    }
}

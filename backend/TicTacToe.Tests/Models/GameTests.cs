using TicTacToe.Api.Models;
using Xunit;

namespace TicTacToe.Tests;

public class GameTests
{
    [Fact]
    public void ValidMove_PlacesPlayerOnBoard()
    {
        var game = new Game(GameMode.TwoPlayer);

        var result = game.Move(Player.X, 0);

        Assert.True(result.Success);
        Assert.Equal(Player.X, game.Board[0]);
    }

    [Fact]
    public void ValidMove_SwitchesCurrentPlayer()
    {
        var game = new Game(GameMode.TwoPlayer);

        game.Move(Player.X, 0);

        Assert.Equal(Player.O, game.CurrentPlayer);
    }

    [Fact]
    public void Move_OnOccupiedCell_IsRejectedAndTurnUnchanged()
    {
        var game = new Game(GameMode.TwoPlayer);
        game.Move(Player.X, 0);

        var result = game.Move(Player.O, 0);

        Assert.False(result.Success);
        Assert.Equal(Player.O, game.CurrentPlayer);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(9)]
    public void Move_OutsideBoard_IsRejected(int cell)
    {
        var game = new Game(GameMode.TwoPlayer);

        var result = game.Move(Player.X, cell);

        Assert.False(result.Success);
    }

    [Fact]
    public void Move_ByWrongPlayer_IsRejected()
    {
        var game = new Game(GameMode.TwoPlayer);

        var result = game.Move(Player.O, 0);

        Assert.False(result.Success);
        Assert.Null(game.Board[0]);
    }

    [Fact]
    public void Move_AfterGameCompleted_IsRejected()
    {
        var game = new Game(GameMode.TwoPlayer);
        PlayRowWinForX(game);

        var result = game.Move(Player.O, 8);

        Assert.False(result.Success);
    }

    [Fact]
    public void RowWin_IsDetected()
    {
        var game = new Game(GameMode.TwoPlayer);
        PlayRowWinForX(game);

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(new[] { 0, 1, 2 }, game.WinningCells);
    }

    [Fact]
    public void ColumnWin_IsDetected()
    {
        var game = new Game(GameMode.TwoPlayer);
        game.Move(Player.X, 0); // X
        game.Move(Player.O, 1); // O
        game.Move(Player.X, 3); // X
        game.Move(Player.O, 4); // O
        game.Move(Player.X, 6); // X wins column 0,3,6

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Player.X, game.Winner);
        Assert.Equal(new[] { 0, 3, 6 }, game.WinningCells);
    }

    [Fact]
    public void DiagonalWin_IsDetected()
    {
        var game = new Game(GameMode.TwoPlayer);
        game.Move(Player.X, 0);
        game.Move(Player.O, 1);
        game.Move(Player.X, 4);
        game.Move(Player.O, 2);
        game.Move(Player.X, 8);

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(new[] { 0, 4, 8 }, game.WinningCells);
    }

    [Fact]
    public void FullBoardWithNoWinner_IsDraw()
    {
        var game = new Game(GameMode.TwoPlayer);
        // X O X
        // X O O
        // O X X
        int[] xMoves = { 0, 2, 3, 7, 8 };
        int[] oMoves = { 1, 4, 5, 6 };
        for (var i = 0; i < oMoves.Length; i++)
        {
            game.Move(Player.X, xMoves[i]);
            game.Move(Player.O, oMoves[i]);
        }
        game.Move(Player.X, xMoves[^1]);

        Assert.Equal(GameStatus.Draw, game.Status);
        Assert.Null(game.Winner);
    }

    [Fact]
    public void Reset_ClearsBoardHistoryAndRestoresPlayerX()
    {
        var game = new Game(GameMode.TwoPlayer);
        PlayRowWinForX(game);

        game.Reset();

        Assert.All(game.Board, cell => Assert.Null(cell));
        Assert.Empty(game.History);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Null(game.Winner);
        Assert.Equal(Player.X, game.CurrentPlayer);
    }

    [Fact]
    public void Undo_TwoPlayerMode_RemovesOnlyLastMove()
    {
        var game = new Game(GameMode.TwoPlayer);
        game.Move(Player.X, 0);
        game.Move(Player.O, 4);

        var result = game.Undo();

        Assert.True(result.Success);
        Assert.Null(game.Board[4]);
        Assert.Equal(Player.X, game.Board[0]);
        Assert.Equal(Player.O, game.CurrentPlayer);
        Assert.Single(game.History);
    }

    [Fact]
    public void Undo_ComputerMode_RemovesComputerAndHumanMoveTogether()
    {
        var game = new Game(GameMode.VsComputer);
        game.Move(Player.X, 0);
        game.Move(Player.O, 4);

        var result = game.Undo();

        Assert.True(result.Success);
        Assert.Null(game.Board[0]);
        Assert.Null(game.Board[4]);
        Assert.Equal(Player.X, game.CurrentPlayer);
        Assert.Empty(game.History);
    }

    [Fact]
    public void Undo_WithNoMoves_IsDisabled()
    {
        var game = new Game(GameMode.TwoPlayer);

        var result = game.Undo();

        Assert.False(result.Success);
    }

    [Fact]
    public void Undo_AfterGameCompletion_IsDisabled()
    {
        var game = new Game(GameMode.TwoPlayer);
        PlayRowWinForX(game);

        var result = game.Undo();

        Assert.False(result.Success);
        Assert.Equal(GameStatus.Won, game.Status);
    }

    private static void PlayRowWinForX(Game game)
    {
        game.Move(Player.X, 0);
        game.Move(Player.O, 3);
        game.Move(Player.X, 1);
        game.Move(Player.O, 4);
        game.Move(Player.X, 2); // X wins row 0,1,2
    }
}

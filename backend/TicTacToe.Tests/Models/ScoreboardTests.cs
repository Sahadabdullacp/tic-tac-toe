using TicTacToe.Api.Models;
using Xunit;

namespace TicTacToe.Tests;

public class ScoreboardTests
{
    [Fact]
    public void NewScoreboard_StartsAtZero()
    {
        var scoreboard = new Scoreboard();

        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void RecordWin_IncrementsCorrectPlayerCount()
    {
        var scoreboard = new Scoreboard();

        scoreboard.RecordResult(GameStatus.Won, Player.X);

        Assert.Equal(1, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
    }

    [Fact]
    public void RecordDraw_IncrementsDrawCount()
    {
        var scoreboard = new Scoreboard();

        scoreboard.RecordResult(GameStatus.Draw, null);

        Assert.Equal(1, scoreboard.Draws);
    }

    [Fact]
    public void RecordResult_CalledTwiceForSameGame_CountsOnce()
    {
        var scoreboard = new Scoreboard();
        var game = new Game(GameMode.TwoPlayer);
        game.Move(Player.X, 0);
        game.Move(Player.O, 3);
        game.Move(Player.X, 1);
        game.Move(Player.O, 4);
        game.Move(Player.X, 2); // X wins

        scoreboard.RecordGameOnce(game);
        scoreboard.RecordGameOnce(game);

        Assert.Equal(1, scoreboard.XWins);
    }

    [Fact]
    public void Reset_ClearsAllCounts()
    {
        var scoreboard = new Scoreboard();
        scoreboard.RecordResult(GameStatus.Won, Player.X);

        scoreboard.Reset();

        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }
}

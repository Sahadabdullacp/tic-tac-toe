namespace TicTacToe.Api.Models;

public enum Player { X, O }

public enum GameMode { TwoPlayer, VsComputer }

public enum GameStatus { InProgress, Won, Draw }

public record Move(int Number, Player Player, int Cell);

public record MoveResult(bool Success, string? Error = null);

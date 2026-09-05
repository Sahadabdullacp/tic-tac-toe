using Microsoft.AspNetCore.Http.HttpResults;
using TicTacToe.Api.Dtos;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Endpoints;

/// <summary>Maps REST endpoints for game and scoreboard operations onto the <see cref="GameService"/>.</summary>
public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("/api");

        api.MapPost("/games", CreateGame);
        api.MapGet("/games/{id:guid}", GetGame);
        api.MapPost("/games/{id:guid}/moves", SubmitMove);
        api.MapPost("/games/{id:guid}/undo", UndoMove);
        api.MapPost("/games/{id:guid}/reset", ResetGame);
        api.MapGet("/scoreboard", GetScoreboard);
        api.MapPost("/scoreboard/reset", ResetScoreboard);
    }

    private static Ok<GameStateDto> CreateGame(CreateGameRequest request, GameService games) =>
        TypedResults.Ok(games.CreateGame(request.Mode).ToDto(games.Scoreboard));

    private static Results<Ok<GameStateDto>, NotFound> GetGame(Guid id, GameService games) =>
        games.FindGame(id) is { } game
            ? TypedResults.Ok(game.ToDto(games.Scoreboard))
            : TypedResults.NotFound();

    private static Results<Ok<GameStateDto>, NotFound, BadRequest<object>> SubmitMove(
        Guid id, MoveRequest request, GameService games)
    {
        if (games.FindGame(id) is not { } game) return TypedResults.NotFound();

        var result = games.SubmitMove(game, request.Player, request.Cell);
        return result.Success
            ? TypedResults.Ok(game.ToDto(games.Scoreboard))
            : TypedResults.BadRequest(ErrorBody(result));
    }

    private static Results<Ok<GameStateDto>, NotFound, BadRequest<object>> UndoMove(Guid id, GameService games)
    {
        if (games.FindGame(id) is not { } game) return TypedResults.NotFound();

        var result = games.Undo(game);
        return result.Success
            ? TypedResults.Ok(game.ToDto(games.Scoreboard))
            : TypedResults.BadRequest(ErrorBody(result));
    }

    private static Results<Ok<GameStateDto>, NotFound> ResetGame(Guid id, GameService games)
    {
        if (games.FindGame(id) is not { } game) return TypedResults.NotFound();

        games.ResetGame(game);
        return TypedResults.Ok(game.ToDto(games.Scoreboard));
    }

    private static Ok<ScoreboardDto> GetScoreboard(GameService games) =>
        TypedResults.Ok(games.Scoreboard.ToDto());

    private static Ok<ScoreboardDto> ResetScoreboard(GameService games)
    {
        games.ResetScoreboard();
        return TypedResults.Ok(games.Scoreboard.ToDto());
    }

    private static object ErrorBody(MoveResult result) => new { error = result.Error };
}

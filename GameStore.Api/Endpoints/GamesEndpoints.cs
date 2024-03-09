using System.Diagnostics;
using GameStore.Api.Authorization;
using GameStore.Api.DTOs;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndPointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {

        var gamesGroup = routes.MapGroup("/games").WithParameterValidation();

        gamesGroup.MapGet("/", async (IGamesRepository repository, ILoggerFactory loggerFactory) =>
        {
            try
            {
                return Results.Ok((await repository.GetAllAsync()).Select(game => game.AsDTO()));
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger("Games Endpoints");
                logger.LogError(ex, "Could not process a request on machine {Machine}. TraceId:{TraceId}",
                Environment.MachineName,
                Activity.Current?.TraceId);

                return Results.Problem(
                    title: "We made a mistake but we're working on it!",
                    statusCode: StatusCodes.Status500InternalServerError,
                    extensions: new Dictionary<string, object?>{
                        {"traceId",Activity.Current?.TraceId.ToString()}
                    }
                );
            }
        });

        gamesGroup
            .MapGet(
                "/{id}",
                async (IGamesRepository repository, int id) =>
                {
                    Game? game = await repository.GetAsync(id);
                    return game is not null ? Results.Ok(game.AsDTO()) : Results.NotFound();
                }
            )
            .WithName(GetGameEndPointName)
            .RequireAuthorization(Policies.ReadAccess);

        gamesGroup.MapPost(
            "/",
            async (IGamesRepository repository, CreateGameDTO gameDTO) =>
            {
                Game game = new()
                {
                    Name = gameDTO.Name,
                    Genre = gameDTO.Genre,
                    Price = gameDTO.Price,
                    ReleaseDate = gameDTO.ReleaseDate,
                    ImageUri = gameDTO.ImageUri
                };
                await repository.CreateAsync(game);

                return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
            }
        ).RequireAuthorization(Policies.WriteAccess);

        gamesGroup.MapPut(
            "/{id}",
            async (IGamesRepository repository, int id, UpdateGameDTO updatedGameDTO) =>
            {
                Game? existingGame = await repository.GetAsync(id);
                if (existingGame is null)
                {
                    return Results.NotFound();
                }

                existingGame.Name = updatedGameDTO.Name;
                existingGame.Genre = updatedGameDTO.Genre;
                existingGame.Price = updatedGameDTO.Price;
                existingGame.ReleaseDate = updatedGameDTO.ReleaseDate;
                existingGame.ImageUri = existingGame.ImageUri;

                await repository.UpdateAsync(existingGame);

                return Results.NoContent();
            }
        ).RequireAuthorization(Policies.WriteAccess);

        gamesGroup.MapDelete(
            "/{id}",
            async (IGamesRepository repository, int id) =>
            {
                Game? existingGame = await repository.GetAsync(id);
                if (existingGame is not null)
                {
                    await repository.DeleteAsync(id);
                }
                return Results.NoContent();
            }
        ).RequireAuthorization(Policies.WriteAccess);
        return gamesGroup;
    }
}

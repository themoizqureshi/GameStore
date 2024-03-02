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

        gamesGroup.MapGet("/", async (IGamesRepository repository) =>
        (await repository.GetAllAsync())
        .Select(game => game.AsDTO())
        );

        gamesGroup
            .MapGet(
                "/{id}",
                async (IGamesRepository repository, int id) =>
                {
                    Game? game = await repository.GetAsync(id);
                    return game is not null ? Results.Ok(game.AsDTO()) : Results.NotFound();
                }
            )
            .WithName(GetGameEndPointName);

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
        );

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
        );

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
        );
        return gamesGroup;
    }
}

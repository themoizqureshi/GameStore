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

        gamesGroup.MapGet("/", (IGamesRepository repository) =>
        repository.GetAll()
        .Select(game => game.AsDTO())
        );

        gamesGroup
            .MapGet(
                "/{id}",
                (IGamesRepository repository, int id) =>
                {
                    Game? game = repository.Get(id);
                    return game is not null ? Results.Ok(game.AsDTO()) : Results.NotFound();
                }
            )
            .WithName(GetGameEndPointName);

        gamesGroup.MapPost(
            "/",
            (IGamesRepository repository, CreateGameDTO gameDTO) =>
            {
                Game game = new()
                {
                    Name = gameDTO.Name,
                    Genre = gameDTO.Genre,
                    Price = gameDTO.Price,
                    ReleaseDate = gameDTO.ReleaseDate,
                    ImageUri = gameDTO.ImageUri
                };
                repository.Create(game);

                return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
            }
        );

        gamesGroup.MapPut(
            "/{id}",
            (IGamesRepository repository, int id, UpdateGameDTO updatedGameDTO) =>
            {
                Game? existingGame = repository.Get(id);
                if (existingGame is null)
                {
                    return Results.NotFound();
                }

                existingGame.Name = updatedGameDTO.Name;
                existingGame.Genre = updatedGameDTO.Genre;
                existingGame.Price = updatedGameDTO.Price;
                existingGame.ReleaseDate = updatedGameDTO.ReleaseDate;
                existingGame.ImageUri = existingGame.ImageUri;

                repository.Update(existingGame);

                return Results.NoContent();
            }
        );

        gamesGroup.MapDelete(
            "/{id}",
            (IGamesRepository repository, int id) =>
            {
                Game? existingGame = repository.Get(id);
                if (existingGame is not null)
                {
                    repository.Delete(id);
                }
                return Results.NoContent();
            }
        );
        return gamesGroup;
    }
}

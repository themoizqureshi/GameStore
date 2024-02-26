using GameStore.Api.Entities;
using GameStore.Api.Repositories;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndPointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {

        var gamesGroup = routes.MapGroup("/games").WithParameterValidation();

        gamesGroup.MapGet("/", (IGamesRepository repository) => repository.GetAll());

        gamesGroup
            .MapGet(
                "/{id}",
                (IGamesRepository repository, int id) =>
                {
                    Game? game = repository.Get(id);
                    return game is not null ? Results.Ok(game) : Results.NotFound();
                }
            )
            .WithName(GetGameEndPointName);

        gamesGroup.MapPost(
            "/",
            (IGamesRepository repository, Game game) =>
            {
                repository.Create(game);

                return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
            }
        );

        gamesGroup.MapPut(
            "/{id}",
            (IGamesRepository repository, int id, Game updatedGame) =>
            {
                Game? existingGame = repository.Get(id);
                if (existingGame is null)
                {
                    return Results.NotFound();
                }

                existingGame.Name = updatedGame.Name;
                existingGame.Genre = updatedGame.Genre;
                existingGame.Price = updatedGame.Price;
                existingGame.ReleaseDate = updatedGame.ReleaseDate;
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

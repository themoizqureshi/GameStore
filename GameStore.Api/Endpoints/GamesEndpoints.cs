using System.Diagnostics;
using GameStore.Api.Authorization;
using GameStore.Api.DTOs;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameV1EndPointName = "GetGameV1";
    const string GetGameV2EndPointName = "GetGameV2";

    public static RouteGroupBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {

        var v1GamesGroup = routes.MapGroup("/v1/games").WithParameterValidation();
        var v2GamesGroup = routes.MapGroup("/v2/games").WithParameterValidation();

        //V1 Get Endpoints
        v1GamesGroup.MapGet("/", async (IGamesRepository repository, ILoggerFactory loggerFactory) =>
        {
            return Results.Ok((await repository.GetAllAsync()).Select(game => game.AsDTOV1()));
        });

        v1GamesGroup
            .MapGet(
                "/{id}",
                async (IGamesRepository repository, int id) =>
                {
                    Game? game = await repository.GetAsync(id);
                    return game is not null ? Results.Ok(game.AsDTOV1()) : Results.NotFound();
                }
            )
            .WithName(GetGameV1EndPointName)
            .RequireAuthorization(Policies.ReadAccess);

        //V2 Get Endpoints       
        v2GamesGroup.MapGet("/", async (IGamesRepository repository, ILoggerFactory loggerFactory) =>
        {
            return Results.Ok((await repository.GetAllAsync()).Select(game => game.AsDTOV2()));
        });

        v2GamesGroup
            .MapGet(
                "/{id}",
                async (IGamesRepository repository, int id) =>
                {
                    Game? game = await repository.GetAsync(id);
                    return game is not null ? Results.Ok(game.AsDTOV2()) : Results.NotFound();
                }
            )
            .WithName(GetGameV2EndPointName)
            .RequireAuthorization(Policies.ReadAccess);

        v1GamesGroup.MapPost(
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

                return Results.CreatedAtRoute(GetGameV1EndPointName, new { id = game.Id }, game);
            }
        ).RequireAuthorization(Policies.WriteAccess);

        v1GamesGroup.MapPut(
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

        v1GamesGroup.MapDelete(
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
        return v1GamesGroup;
    }
}

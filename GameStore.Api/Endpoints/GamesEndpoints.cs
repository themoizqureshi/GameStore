using GameStore.Api.Authorization;
using GameStore.Api.DTOs;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameV1EndPointName = "GetGameV1";
    const string GetGameV2EndPointName = "GetGameV2";

    public static RouteGroupBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {

        var gamesGroup = routes.NewVersionedApi()
        .MapGroup("/games")
        .HasApiVersion(1.0)
        .HasApiVersion(2.0)
        .WithParameterValidation()
        .WithOpenApi()
        .WithTags("Games");
        //V1 Get Endpoints      
        gamesGroup.MapGet("/", async (
            IGamesRepository repository,
            ILoggerFactory loggerFactory,
            [AsParameters] GetGamesDTOV1 request,
            HttpContext http) =>
        {
            var totalCount = await repository.CountAsync(request.filter);
            http.Response.AddPaginationHeader(totalCount, request.pageSize);
            return Results.Ok((await repository.GetAllAsync(request.pageNumber, request.pageSize, request.filter))
                                                .Select(game => game.AsDTOV1()));
        }).MapToApiVersion(1.0)
        .WithSummary("Gets all games")
        .WithDescription("Gets all available games and allows filtering and pagination");

        gamesGroup
            .MapGet(
                "/{id}",
                async Task<Results<Ok<GameDTOV1>, NotFound>> (IGamesRepository repository, int id) =>
                {
                    Game? game = await repository.GetAsync(id);
                    return game is not null ? TypedResults.Ok(game.AsDTOV1()) : TypedResults.NotFound();
                }
            )
            .WithName(GetGameV1EndPointName)
            .RequireAuthorization(Policies.ReadAccess).MapToApiVersion(1.0)
            .WithSummary("Gets a game by id")
            .WithDescription("Gets the game that has the specified id");

        //V2 Get Endpoints       
        gamesGroup.MapGet("/", async (
            IGamesRepository repository,
            ILoggerFactory loggerFactory,
            [AsParameters] GetGamesDTOV2 request,
            HttpContext http) =>
        {
            var totalCount = await repository.CountAsync(request.filter);
            http.Response.AddPaginationHeader(totalCount, request.pageSize);

            return Results.Ok((await repository.GetAllAsync(
                request.pageNumber,
                request.pageSize,
                request.filter))
            .Select(game => game.AsDTOV2()));
        }).MapToApiVersion(2.0)
        .WithSummary("Gets all games")
        .WithDescription("Gets all available games and allows filtering and pagination");

        gamesGroup
            .MapGet(
                "/{id}",
                async Task<Results<Ok<GameDTOV2>, NotFound>> (IGamesRepository repository, int id) =>
                {
                    Game? game = await repository.GetAsync(id);
                    return game is not null ? TypedResults.Ok(game.AsDTOV2()) : TypedResults.NotFound();
                }
            )
            .WithName(GetGameV2EndPointName)
            .RequireAuthorization(Policies.ReadAccess)
            .MapToApiVersion(2.0)
            .WithSummary("Gets a game by id")
            .WithDescription("Gets the game that has the specified id");

        gamesGroup.MapPost(
            "/",
            async Task<CreatedAtRoute<GameDTOV1>> (IGamesRepository repository, CreateGameDTO gameDTO) =>
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

                return TypedResults.CreatedAtRoute(game.AsDTOV1(), GetGameV1EndPointName, new { id = game.Id });
            }
        ).RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0)
        .WithSummary("Creates a new game")
        .WithDescription("Creates a new game with the specified properties");

        gamesGroup.MapPut(
            "/{id}",
            async Task<Results<NotFound, NoContent>> (IGamesRepository repository, int id, UpdateGameDTO updatedGameDTO) =>
            {
                Game? existingGame = await repository.GetAsync(id);
                if (existingGame is null)
                {
                    return TypedResults.NotFound();
                }

                existingGame.Name = updatedGameDTO.Name;
                existingGame.Genre = updatedGameDTO.Genre;
                existingGame.Price = updatedGameDTO.Price;
                existingGame.ReleaseDate = updatedGameDTO.ReleaseDate;
                existingGame.ImageUri = existingGame.ImageUri;

                await repository.UpdateAsync(existingGame);

                return TypedResults.NoContent();
            }
        ).RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0)
        .WithSummary("Updates a game")
        .WithDescription("Updates all game properties for the fame that has the specified id");

        gamesGroup.MapDelete(
            "/{id}",
            async (IGamesRepository repository, int id) =>
            {
                Game? existingGame = await repository.GetAsync(id);
                if (existingGame is not null)
                {
                    await repository.DeleteAsync(id);
                }
                return TypedResults.NoContent();
            }
        ).RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0)
        .WithSummary("Deletes a game")
        .WithDescription("Deletes the game that has the specified id");
        return gamesGroup;
    }
}

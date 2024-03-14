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

        var gamesGroup = routes.NewVersionedApi()
        .MapGroup("/games")
        .HasApiVersion(1.0)
        .HasApiVersion(2.0)
        .WithParameterValidation();
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
        }).MapToApiVersion(1.0);

        gamesGroup
            .MapGet(
                "/{id}",
                async (IGamesRepository repository, int id) =>
                {
                    Game? game = await repository.GetAsync(id);
                    return game is not null ? Results.Ok(game.AsDTOV1()) : Results.NotFound();
                }
            )
            .WithName(GetGameV1EndPointName)
            .RequireAuthorization(Policies.ReadAccess).MapToApiVersion(1.0);

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
        }).MapToApiVersion(2.0);

        gamesGroup
            .MapGet(
                "/{id}",
                async (IGamesRepository repository, int id) =>
                {
                    Game? game = await repository.GetAsync(id);
                    return game is not null ? Results.Ok(game.AsDTOV2()) : Results.NotFound();
                }
            )
            .WithName(GetGameV2EndPointName)
            .RequireAuthorization(Policies.ReadAccess).MapToApiVersion(2.0);

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

                return Results.CreatedAtRoute(GetGameV1EndPointName, new { id = game.Id }, game);
            }
        ).RequireAuthorization(Policies.WriteAccess).MapToApiVersion(1.0);

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
        ).RequireAuthorization(Policies.WriteAccess).MapToApiVersion(1.0);

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
        ).RequireAuthorization(Policies.WriteAccess).MapToApiVersion(1.0);
        return gamesGroup;
    }
}

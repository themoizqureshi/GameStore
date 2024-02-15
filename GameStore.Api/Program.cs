using GameStore.Api.Entities;
const string GetGameEndPointName = "GetGame";

List<Game> games = new(){
    new Game(){
        Id = 1,
        Name = "Street Fighter II",
        Genre = "Fighting",
        Price = 19.99M,
        ReleaseDate = new DateTime(1991,2,1),
        ImageUri = "https://placehold.co/100",
    },
    new Game(){
        Id = 2,
        Name = "Final Fantasy XIV",
        Genre = "Roleplaying",
        Price = 59.99M,
        ReleaseDate = new DateTime(2010,9,28),
        ImageUri = "https://placehold.co/100",
    },
    new Game(){
        Id = 3,
        Name = "FIFA 23",
        Genre = "Sports",
        Price = 69.99M,
        ReleaseDate = new DateTime(2022,9,27),
        ImageUri = "https://placehold.co/100",
    },
};


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var gamesGroup = app.MapGroup("/games");


gamesGroup.MapGet("/", () => games);

gamesGroup.MapGet("/{id}", (int id) =>
{
    Game? game = games.Find(game => game.Id == id);
    if (game is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(game);
}
).WithName(GetGameEndPointName);

gamesGroup.MapPost("/", (Game game) =>
{
    game.Id = games.Max(game => game.Id) + 1;
    games.Add(game);

    return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
});

gamesGroup.MapPut("/{id}", (int id, Game updatedGame) =>
{
    Game? existingGame = games.Find(game => game.Id == id);
    if (existingGame is null)
    {
        return Results.NotFound();
    }

    existingGame.Name = updatedGame.Name;
    existingGame.Genre = updatedGame.Genre;
    existingGame.Price = updatedGame.Price;
    existingGame.ReleaseDate = updatedGame.ReleaseDate;
    existingGame.ImageUri = existingGame.ImageUri;

    return Results.NoContent();
});

gamesGroup.MapDelete("/{id}", (int id) =>
{
    Game? existingGame = games.Find(game => game.Id == id);
    if (existingGame is not null)
    {
        games.Remove(existingGame);
    }
    return Results.NoContent();

});

app.Run();

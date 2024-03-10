using GameStore.Api.Authorization;
using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using GameStore.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRepositories(builder.Configuration);

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddGameStoreAuthorization();
builder.Services.AddHttpLogging(o => { });

// builder.Logging.AddJsonConsole(options =>
// {
//     options.JsonWriterOptions = new()
//     {
//         Indented = true
//     };
// });

var app = builder.Build();

app.UseMiddleware<RequestTimingMiddleware>();

await app.Services.InitializeDb();

app.UseHttpLogging();
app.MapGamesEndpoints();

app.Run();

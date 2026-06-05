using PuzzleGames.API.Data;
using PuzzleGames.API.Endpoints;
using PuzzleGames.API.Services;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("PuzzleGames");
builder.Services.AddSqlite<PuzzleGamesContext>(connString);
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IUserGamesProgressService,UserGamesProgressService>();

var app = builder.Build();

app.MapUsersEndpoints();
app.MapUserGamesProgressEndpoints();

await app.MigrateDbAsync();

app.Run();

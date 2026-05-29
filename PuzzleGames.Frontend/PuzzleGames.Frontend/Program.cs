using PuzzleGames.Frontend.Clients;
using PuzzleGames.Frontend.Components;
using PuzzleGames.Frontend.Logic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var puzzleGamesApiUrl = builder.Configuration["PuzzleGamesApiUrl"]??
                      throw new Exception("GameStoreApiUrl is not set");
builder.Services.AddHttpClient<UserClient>(client => client.BaseAddress = new Uri(puzzleGamesApiUrl));
builder.Services.AddScoped<UserSession>();
builder.Services.AddScoped<TakuzuGameLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
using Microsoft.EntityFrameworkCore;

namespace PuzzleGames.API.Data;

public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PuzzleGamesContext>();
        await dbContext.Database.MigrateAsync();
    }
}
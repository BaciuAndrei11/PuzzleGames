using Microsoft.EntityFrameworkCore;
using PuzzleGames.API.Data;
using PuzzleGames.API.Dtos;
using PuzzleGames.API.Entities;
using PuzzleGames.API.Enums;

namespace PuzzleGames.API.Services;

public class UserGamesProgressService(PuzzleGamesContext dbContext) : IUserGamesProgressService
{
    public async Task UpdateGameProgressAsync(int userId, UpdateProgressDto updateProgressDto)
    {
        var progress = await dbContext.UserGameProgresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.GameType == updateProgressDto.GameType);

        if (progress is null)
        {
            var newProgress = new UserGameProgress
            {
                UserId = userId,
                GameType = updateProgressDto.GameType,
                CurrentLevel = updateProgressDto.NewLevel
            };
            await dbContext.UserGameProgresses.AddAsync(newProgress);
        }
        else
        {
            if (updateProgressDto.NewLevel > progress.CurrentLevel)
            {
                progress.CurrentLevel = updateProgressDto.NewLevel;
            }
        }
        await dbContext.SaveChangesAsync();
    }
}
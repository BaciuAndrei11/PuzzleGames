using PuzzleGames.API.Dtos;
using PuzzleGames.API.Enums;

namespace PuzzleGames.API.Services;

public interface IUserGamesProgressService
{
    Task UpdateGameProgressAsync(int userId, UpdateProgressDto updateProgressDto);
}
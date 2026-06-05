using PuzzleGames.API.Dtos;
using PuzzleGames.API.Enums;
using PuzzleGames.API.Services;

namespace PuzzleGames.API.Endpoints;

public static class UserGameProgressEndpoints
{
    public static RouteGroupBuilder MapUserGamesProgressEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("gamesProgress")
            .WithParameterValidation();

        // PUT /gamesProgress/5
        group.MapPut("/{userId}", async (int userId, UpdateProgressDto updateProgressDto, IUserGamesProgressService progressService) =>
            {
                await progressService.UpdateGameProgressAsync(userId, updateProgressDto);
            
                return Results.NoContent();
            });
        
        return group;
    }
}
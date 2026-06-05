namespace PuzzleGames.Frontend.Models;

public record UpdateProgress(
    PuzzleGames.API.Enums.GameType GameType, 
    int NewLevel
);
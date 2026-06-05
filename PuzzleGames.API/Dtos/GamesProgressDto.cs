using PuzzleGames.API.Enums;

namespace PuzzleGames.API.Dtos;

public record GamesProgressDto(
    int Id,
    GameType GameType,
    int CurrentLevel);
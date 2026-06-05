namespace PuzzleGames.API.Dtos;

public record UserDto(
    int Id,
    string Username,
    string Email,
    string Password,
    IEnumerable<GamesProgressDto> GameProgresses);
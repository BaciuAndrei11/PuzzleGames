using PuzzleGames.API.Enums;

namespace PuzzleGames.API.Entities;

public class UserGameProgress
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public GameType GameType { get; set; }

    public int CurrentLevel { get; set; } = 1;
}
namespace PuzzleGames.Frontend.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<GamesProgress> GameProgresses  { get; set; }
    
    public int GetLevelFor(GameType gameType)
    {
        return GameProgresses?
            .FirstOrDefault(p => p.GameType.Equals(gameType))
            ?.CurrentLevel ?? 1;
    }
}
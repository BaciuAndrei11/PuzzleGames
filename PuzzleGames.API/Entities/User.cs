namespace PuzzleGames.API.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    
    public ICollection<UserGameProgress> GameProgresses { get; set; } = new List<UserGameProgress>();
}
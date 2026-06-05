namespace PuzzleGames.Frontend.Models;

public class GamesProgress
{
    public int Id { get; set; }
    public GameType GameType { get; set; }
    public int CurrentLevel{ get; set; }
}
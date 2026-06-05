namespace PuzzleGames.Frontend.Models;

public class MoveAction
{
    public int Row { get; set; }
    public int Col { get; set; }
    public int PreviousValue { get; set; }
    public int NewValue { get; set; }
    
}
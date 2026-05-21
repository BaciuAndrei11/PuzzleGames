namespace PuzzleGames.Frontend.Models;

public class MoveAction
{
    public int Row { get; set; }
    public int Col { get; set; }
    public TakuzuCellEnum PreviousValue { get; set; }
    public TakuzuCellEnum NewValue { get; set; }
    
}
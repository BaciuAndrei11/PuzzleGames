namespace PuzzleGames.Frontend.Models;

public class TakuzuCell
{
    public TakuzuCell(TakuzuEnum cell)
    {
        Cell = cell;
    }
    public TakuzuEnum Cell { get; set; }
}
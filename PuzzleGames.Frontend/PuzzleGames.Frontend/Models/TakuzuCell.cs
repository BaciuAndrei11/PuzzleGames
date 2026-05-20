namespace PuzzleGames.Frontend.Models;

public class TakuzuCell
{
    public TakuzuCell(TakuzuCellEnum cell, TakuzuLineEnum line)
    {
        Cell = cell;
        LineValue = line;
    }
    public TakuzuCellEnum Cell { get; set; }
    public TakuzuLineEnum LineValue { get; set; }
}
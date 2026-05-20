namespace PuzzleGames.Frontend.Models;

public class TakuzuCell
{
    public TakuzuCell(TakuzuCellEnum cell, TakuzuLineEnum line, bool isFixed)
    {
        Cell = cell;
        LineValue = line;
        IsFixed = isFixed;
    }
    public TakuzuCellEnum Cell { get; set; }
    public TakuzuLineEnum LineValue { get; set; }
    public bool IsFixed { get; set; }
}
namespace PuzzleGames.Frontend.Models;

public class TakuzuCell
{
    public TakuzuCell()
    {
        Cell = TakuzuCellEnum.Empty;
        HorizontalLineValue = TakuzuLineEnum.None;
        VerticalLineValue = TakuzuLineEnum.None;
        IsFixed = false;
    }
    public TakuzuCellEnum Cell { get; set; }
    public TakuzuLineEnum HorizontalLineValue { get; set; }
    public TakuzuLineEnum VerticalLineValue { get; set; }
    public bool IsFixed { get; set; }
}
namespace PuzzleGames.Frontend.Models;

public class StarBattleCell
{
    public StarBattleCell(StarBattleCellEnum cell, StarBattleCellColorEnum color)
    {
        Cell = cell;
        Color = color;
    }
    public StarBattleCellEnum Cell { get; set; }
    public StarBattleCellColorEnum Color { get; set; }

    public string ConvertColor()
    {
        return Color switch
        {
            StarBattleCellColorEnum.Red => "#FF6B6B",
            StarBattleCellColorEnum.Orange => "#FF9F43",
            StarBattleCellColorEnum.Yellow => "#FECA57",
            StarBattleCellColorEnum.Green => "#1DD1A1",
            StarBattleCellColorEnum.Blue => "#54A0FF",
            StarBattleCellColorEnum.Purple => "#5F27CD",
            StarBattleCellColorEnum.Gray => "#A4B0BE",
            StarBattleCellColorEnum.Pink => "#FF9FF3",
            _ => "#FFFFFF" 
        };
    }
}
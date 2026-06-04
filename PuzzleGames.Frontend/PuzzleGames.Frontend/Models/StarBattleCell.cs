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
        switch (Color)
        {
            case StarBattleCellColorEnum.Red:
                return "#FF0000";
            case StarBattleCellColorEnum.Orange:
                return "#FFA500";
            case StarBattleCellColorEnum.Yellow:
                return "#FFFF00";
            case StarBattleCellColorEnum.Green:
                return "#00FF00";
            case StarBattleCellColorEnum.Blue:
                return "#0000FF";
            case StarBattleCellColorEnum.Purple:
                return "#8F00FF";
            case StarBattleCellColorEnum.Gray:
                return "#808080";
            case StarBattleCellColorEnum.Pink:
                return "#FFC0CB";
            default:
                return "";
                
        }
    }
}
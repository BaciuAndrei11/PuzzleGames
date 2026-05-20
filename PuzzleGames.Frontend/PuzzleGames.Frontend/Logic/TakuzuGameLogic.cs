using PuzzleGames.Frontend.Models;

namespace PuzzleGames.Frontend.Logic;

public class TakuzuGameLogic
{
    public List<List<TakuzuCell>> TakuzuBoard { get; set; }
    public int Rows { get; set; }
    public int Cols { get; set; }

    public TakuzuGameLogic(List<List<TakuzuCell>> takuzuBoard)
    {
        if (takuzuBoard.Count == takuzuBoard[0].Count)
        {
            TakuzuBoard = takuzuBoard;
            Rows = takuzuBoard.Count;
            Cols = takuzuBoard[0].Count;
        }
    }

    public void ChangeCellValue(int row, int col)
    {
        switch (TakuzuBoard[row][col].Cell) 
        {   
            case TakuzuEnum.Empty:
                TakuzuBoard[row][col].Cell = TakuzuEnum.Water;
                break;
            case TakuzuEnum.Water:
                TakuzuBoard[row][col].Cell = TakuzuEnum.Fire;
                break;
            case TakuzuEnum.Fire:
                TakuzuBoard[row][col].Cell = TakuzuEnum.Empty;
                break;
        }
    }
}
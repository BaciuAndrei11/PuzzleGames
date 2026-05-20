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
        if(TakuzuBoard[row][col].IsFixed == true)
            return;
        switch (TakuzuBoard[row][col].Cell) 
        {   
            case TakuzuCellEnum.Empty:
                TakuzuBoard[row][col].Cell = TakuzuCellEnum.Water;
                break;
            case TakuzuCellEnum.Water:
                TakuzuBoard[row][col].Cell = TakuzuCellEnum.Fire;
                break;
            case TakuzuCellEnum.Fire:
                TakuzuBoard[row][col].Cell = TakuzuCellEnum.Empty;
                break;
        }
    }
}
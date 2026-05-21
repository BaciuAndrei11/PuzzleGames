using PuzzleGames.Frontend.Models;
using PuzzleGames.Frontend.Utilities;

namespace PuzzleGames.Frontend.Logic;

public class TakuzuGameLogic
{
    public List<List<TakuzuCell>> TakuzuBoard { get; set; }
    public List<List<TakuzuCell>> GeneratedBoard { get; set; }
    public int Rows { get; set; }
    public int Cols { get; set; }
    public bool IsGameOver { get; set; }
    private Random _random = new Random();
    

    public TakuzuGameLogic()
    {
    }

    public void GenerateNewGame(int size)
    {
        TakuzuBoard = new List<List<TakuzuCell>>();
        for (int r = 0; r < size; r++)
        {
            var rowList = new List<TakuzuCell>();
            for (int c = 0; c < size; c++)
            {
                rowList.Add(new TakuzuCell());
            }
            TakuzuBoard.Add(rowList);
        }
        Rows = size;
        Cols = size;
        IsGameOver = false;
        
        TakuzuGameUtility.GenerateFullRandomBoard(0, 0, TakuzuBoard);
        TakuzuGameUtility.GenerateHorizontalAndVerticalClues(TakuzuBoard);
        TakuzuGameUtility.GenerateClues(TakuzuBoard);

        GeneratedBoard = TakuzuGameUtility.CloneBoard(TakuzuBoard);
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

        TakuzuGameUtility.ValidateBoard(TakuzuBoard);
        IsGameOver = TakuzuGameUtility.IsGameOver(TakuzuBoard);
    }

    public void ResetBoard()
    {
        TakuzuBoard = TakuzuGameUtility.CloneBoard(GeneratedBoard);
    }
}
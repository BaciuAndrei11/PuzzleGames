using PuzzleGames.Frontend.Models;

namespace PuzzleGames.Frontend.Logic;

public class TakuzuGameLogic
{
    public List<List<TakuzuCell>> TakuzuBoard { get; set; }
    public int Rows { get; set; }
    public int Cols { get; set; }
    
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
        
        GenrateFullRandomBoard(0, 0);
        GenerateAllClues();
        ApplyDifficultyMask();
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
    
    private void ApplyDifficultyMask()
    {
        int cellsToKeep = 4;
        int currentCells = Rows * Cols;

        while (currentCells > cellsToKeep)
        {
            int r = _random.Next(TakuzuBoard.Count);
            int c = _random.Next(TakuzuBoard.Count);

            if (TakuzuBoard[r][c].Cell != TakuzuCellEnum.Empty)
            {
                TakuzuBoard[r][c].Cell = TakuzuCellEnum.Empty;
                currentCells--;
            }
        }

        for (int r = 0; r < TakuzuBoard.Count; r++)
        {
            for (int c = 0; c < TakuzuBoard.Count; c++)
            {
                if (TakuzuBoard[r][c].Cell != TakuzuCellEnum.Empty)
                    TakuzuBoard[r][c].IsFixed = true;
            }
        }

        FilterClues(0.05, true);
        FilterClues(0.05,  false);
    }
    
    private void FilterClues(double visibilityPercentage, bool horizontal)
    {
        int rows = TakuzuBoard.Count;
        int cols = TakuzuBoard[0].Count;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (_random.NextDouble() > visibilityPercentage)
                {
                    if (horizontal)
                    {
                        TakuzuBoard[r][c].HorizontalLineValue = TakuzuLineEnum.None;
                    }
                    else
                    {
                        TakuzuBoard[r][c].VerticalLineValue = TakuzuLineEnum.None;
                    }
                }
            }
        }
    }
    

    private bool GenrateFullRandomBoard(int row, int col)
    {
        if (row == TakuzuBoard.Count) 
            return true;
        
        int nextRow = col == TakuzuBoard.Count - 1 ? row + 1 : row;
        int nextCol = col == TakuzuBoard.Count - 1 ? 0 : col + 1;
        
        var options = _random.Next(2) == 0 
            ? new[] { TakuzuCellEnum.Fire, TakuzuCellEnum.Water } 
            : new[] { TakuzuCellEnum.Water, TakuzuCellEnum.Fire };
        
        foreach (var option in options)
        {
            TakuzuBoard[row][col].Cell = option;

            if (IsValidPlacement(row, col))
            {
                if (GenrateFullRandomBoard(nextRow, nextCol)) return true; 
            }
        }
        
        TakuzuBoard[row][col].Cell = TakuzuCellEnum.Empty;
        return false;
    }
    
    private void GenerateAllClues()
    {
        for (int r = 0; r < TakuzuBoard.Count; r++)
        {
            for (int c = 0; c < TakuzuBoard.Count; c++)
            {
                if (c < TakuzuBoard.Count - 1)
                    TakuzuBoard[r][c].HorizontalLineValue = TakuzuBoard[r][c].Cell == TakuzuBoard[r][c + 1].Cell ? TakuzuLineEnum.Equal : TakuzuLineEnum.Different;
                
                if (r < TakuzuBoard.Count - 1)
                    TakuzuBoard[r][c].VerticalLineValue = TakuzuBoard[r][c].Cell == TakuzuBoard[r + 1][c].Cell ? TakuzuLineEnum.Equal : TakuzuLineEnum.Different;
            }
        }
    }
    
    private bool IsValidPlacement(int row, int col)
    {
        TakuzuCellEnum currentType = TakuzuBoard[row][col].Cell;

        if (col >= 2 && TakuzuBoard[row][col - 1].Cell == currentType && TakuzuBoard[row][col - 2].Cell == currentType) return false;
        if (col >= 1 && col < TakuzuBoard.Count - 1 && TakuzuBoard[row][col - 1].Cell == currentType && TakuzuBoard[row][col + 1].Cell == currentType) return false;
        if (col < TakuzuBoard.Count - 2 && TakuzuBoard[row][col + 1].Cell == currentType && TakuzuBoard[row][col + 2].Cell == currentType) return false;

        if (row >= 2 && TakuzuBoard[row - 1][col].Cell == currentType && TakuzuBoard[row - 2][col].Cell == currentType) return false;
        if (row >= 1 && row < TakuzuBoard.Count - 1 && TakuzuBoard[row - 1][col].Cell == currentType && TakuzuBoard[row + 1][col].Cell == currentType) return false;
        if (row < TakuzuBoard.Count - 2 && TakuzuBoard[row + 1][col].Cell == currentType && TakuzuBoard[row + 2][col].Cell == currentType) return false;

        int maxAllowed = TakuzuBoard.Count / 2;
        
        int rowCount = 0;
        for (int c = 0; c < TakuzuBoard.Count; c++)
        {
            if (TakuzuBoard[row][c].Cell == currentType) 
                rowCount++;
        }
        if (rowCount > maxAllowed) 
            return false;

        int colCount = 0;
        for (int r = 0; r < TakuzuBoard.Count; r++)
        {
            if (TakuzuBoard[r][col].Cell == currentType) 
                colCount++;
        }

        if (colCount > maxAllowed) 
            return false;

        return true;
    }
    
}
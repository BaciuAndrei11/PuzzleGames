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
        GenerateHorizontalAndVerticalClues();
        
        GenerateClues();
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
    private void GenerateClues()
    {
        var allElements = new List<(ClueTypeEnum Type, int R, int C)>();

        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                allElements.Add((ClueTypeEnum.Cell, r, c));

        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols - 1; c++)
                allElements.Add((ClueTypeEnum.Horizontal, r, c));

        for (int r = 0; r < Rows - 1; r++)
            for (int c = 0; c < Cols; c++)
                allElements.Add((ClueTypeEnum.Vertical, r, c));

        allElements = allElements.OrderBy(x => _random.Next()).ToList();

        foreach (var element in allElements)
        {
            if (element.Type == ClueTypeEnum.Cell)
            {
                TakuzuCellEnum originalValue = TakuzuBoard[element.R][element.C].Cell;
                TakuzuBoard[element.R][element.C].Cell = TakuzuCellEnum.Empty;

                if (!CanBeSolvedLogically(CloneBoard(TakuzuBoard)))
                {
                    TakuzuBoard[element.R][element.C].Cell = originalValue;
                    TakuzuBoard[element.R][element.C].IsFixed = true; 
                }
            }
            else if (element.Type == ClueTypeEnum.Horizontal)
            {
                TakuzuLineEnum originalLine = TakuzuBoard[element.R][element.C].HorizontalLineValue;
                TakuzuBoard[element.R][element.C].HorizontalLineValue = TakuzuLineEnum.None;

                if (!CanBeSolvedLogically(CloneBoard(TakuzuBoard)))
                {
                    TakuzuBoard[element.R][element.C].HorizontalLineValue = originalLine;
                }
            }
            else if (element.Type == ClueTypeEnum.Vertical)
            {
                TakuzuLineEnum originalLine = TakuzuBoard[element.R][element.C].VerticalLineValue;
                TakuzuBoard[element.R][element.C].VerticalLineValue = TakuzuLineEnum.None;

                if (!CanBeSolvedLogically(CloneBoard(TakuzuBoard)))
                {
                    TakuzuBoard[element.R][element.C].VerticalLineValue = originalLine;
                }
            }
        }

        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (TakuzuBoard[r][c].Cell != TakuzuCellEnum.Empty)
                    TakuzuBoard[r][c].IsFixed = true;
    }

    private bool CanBeSolvedLogically(List<List<TakuzuCell>> testBoard)
    {
        bool progressMade = true;

        while (progressMade)
        {
            progressMade = false;

            if (ApplyConsecutiveRules(testBoard)) progressMade = true;
            if (ApplyCountingRules(testBoard)) progressMade = true;
            if (ApplyClueRules(testBoard)) progressMade = true;
        }

        return IsBoardComplete(testBoard);
    }

    private bool ApplyConsecutiveRules(List<List<TakuzuCell>> board)
    {
        bool madeProgress = false;

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (c <= Cols - 3)
                {
                    var c1 = board[r][c];
                    var c2 = board[r][c + 1];
                    var c3 = board[r][c + 2];

                    if (c1.Cell != TakuzuCellEnum.Empty && c1.Cell == c2.Cell && c3.Cell == TakuzuCellEnum.Empty)
                    { c3.Cell = GetOpposite(c1.Cell); madeProgress = true; }
                    
                    if (c2.Cell != TakuzuCellEnum.Empty && c2.Cell == c3.Cell && c1.Cell == TakuzuCellEnum.Empty)
                    { c1.Cell = GetOpposite(c2.Cell); madeProgress = true; }
                    
                    if (c1.Cell != TakuzuCellEnum.Empty && c1.Cell == c3.Cell && c2.Cell == TakuzuCellEnum.Empty)
                    { c2.Cell = GetOpposite(c1.Cell); madeProgress = true; }
                }

                if (r <= Rows - 3)
                {
                    var r1 = board[r][c];
                    var r2 = board[r + 1][c];
                    var r3 = board[r + 2][c];

                    if (r1.Cell != TakuzuCellEnum.Empty && r1.Cell == r2.Cell && r3.Cell == TakuzuCellEnum.Empty)
                    { r3.Cell = GetOpposite(r1.Cell); madeProgress = true; }
                    
                    if (r2.Cell != TakuzuCellEnum.Empty && r2.Cell == r3.Cell && r1.Cell == TakuzuCellEnum.Empty)
                    { r1.Cell = GetOpposite(r2.Cell); madeProgress = true; }
                    
                    if (r1.Cell != TakuzuCellEnum.Empty && r1.Cell == r3.Cell && r2.Cell == TakuzuCellEnum.Empty)
                    { r2.Cell = GetOpposite(r1.Cell); madeProgress = true; }
                }
            }
        }
        return madeProgress;
    }

    private bool ApplyCountingRules(List<List<TakuzuCell>> board)
    {
        bool madeProgress = false;
        int maxPerLine = Rows / 2;

        for (int r = 0; r < Rows; r++)
        {
            int waterCount = 0; int fireCount = 0;
            for (int c = 0; c < Cols; c++)
            {
                if (board[r][c].Cell == TakuzuCellEnum.Water) waterCount++;
                if (board[r][c].Cell == TakuzuCellEnum.Fire) fireCount++;
            }

            if (waterCount == maxPerLine && fireCount < maxPerLine)
            {
                for (int c = 0; c < Cols; c++) if (board[r][c].Cell == TakuzuCellEnum.Empty) { board[r][c].Cell = TakuzuCellEnum.Fire; madeProgress = true; }
            }
            else if (fireCount == maxPerLine && waterCount < maxPerLine)
            {
                for (int c = 0; c < Cols; c++) if (board[r][c].Cell == TakuzuCellEnum.Empty) { board[r][c].Cell = TakuzuCellEnum.Water; madeProgress = true; }
            }
        }

        for (int c = 0; c < Cols; c++)
        {
            int waterCount = 0; int fireCount = 0;
            for (int r = 0; r < Rows; r++)
            {
                if (board[r][c].Cell == TakuzuCellEnum.Water) waterCount++;
                if (board[r][c].Cell == TakuzuCellEnum.Fire) fireCount++;
            }

            if (waterCount == maxPerLine && fireCount < maxPerLine)
            {
                for (int r = 0; r < Rows; r++) if (board[r][c].Cell == TakuzuCellEnum.Empty) { board[r][c].Cell = TakuzuCellEnum.Fire; madeProgress = true; }
            }
            else if (fireCount == maxPerLine && waterCount < maxPerLine)
            {
                for (int r = 0; r < Rows; r++) if (board[r][c].Cell == TakuzuCellEnum.Empty) { board[r][c].Cell = TakuzuCellEnum.Water; madeProgress = true; }
            }
        }
        return madeProgress;
    }

    private bool ApplyClueRules(List<List<TakuzuCell>> board)
    {
        bool madeProgress = false;

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (c < Cols - 1 && board[r][c].HorizontalLineValue != TakuzuLineEnum.None)
                {
                    var left = board[r][c];
                    var right = board[r][c + 1];

                    if (board[r][c].HorizontalLineValue == TakuzuLineEnum.Equal)
                    {
                        if (left.Cell != TakuzuCellEnum.Empty && right.Cell == TakuzuCellEnum.Empty) { right.Cell = left.Cell; madeProgress = true; }
                        if (right.Cell != TakuzuCellEnum.Empty && left.Cell == TakuzuCellEnum.Empty) { left.Cell = right.Cell; madeProgress = true; }
                    }
                    else if (board[r][c].HorizontalLineValue == TakuzuLineEnum.Different)
                    {
                        if (left.Cell != TakuzuCellEnum.Empty && right.Cell == TakuzuCellEnum.Empty) { right.Cell = GetOpposite(left.Cell); madeProgress = true; }
                        if (right.Cell != TakuzuCellEnum.Empty && left.Cell == TakuzuCellEnum.Empty) { left.Cell = GetOpposite(right.Cell); madeProgress = true; }
                    }
                }

                if (r < Rows - 1 && board[r][c].VerticalLineValue != TakuzuLineEnum.None)
                {
                    var top = board[r][c];
                    var bottom = board[r + 1][c];

                    if (board[r][c].VerticalLineValue == TakuzuLineEnum.Equal)
                    {
                        if (top.Cell != TakuzuCellEnum.Empty && bottom.Cell == TakuzuCellEnum.Empty) { bottom.Cell = top.Cell; madeProgress = true; }
                        if (bottom.Cell != TakuzuCellEnum.Empty && top.Cell == TakuzuCellEnum.Empty) { top.Cell = bottom.Cell; madeProgress = true; }
                    }
                    else if (board[r][c].VerticalLineValue == TakuzuLineEnum.Different)
                    {
                        if (top.Cell != TakuzuCellEnum.Empty && bottom.Cell == TakuzuCellEnum.Empty) { bottom.Cell = GetOpposite(top.Cell); madeProgress = true; }
                        if (bottom.Cell != TakuzuCellEnum.Empty && top.Cell == TakuzuCellEnum.Empty) { top.Cell = GetOpposite(bottom.Cell); madeProgress = true; }
                    }
                }
            }
        }
        return madeProgress;
    }

    private TakuzuCellEnum GetOpposite(TakuzuCellEnum cellType) =>
        cellType == TakuzuCellEnum.Water ? TakuzuCellEnum.Fire : TakuzuCellEnum.Water;

    private bool IsBoardComplete(List<List<TakuzuCell>> board)
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (board[r][c].Cell == TakuzuCellEnum.Empty) return false;
        return true;
    }

    private List<List<TakuzuCell>> CloneBoard(List<List<TakuzuCell>> original)
    {
        var clone = new List<List<TakuzuCell>>();
        for (int r = 0; r < original.Count; r++)
        {
            var row = new List<TakuzuCell>();
            for (int c = 0; c < original[r].Count; c++)
            {
                row.Add(new TakuzuCell 
                { 
                    Cell = original[r][c].Cell, 
                    IsFixed = original[r][c].IsFixed,
                    HorizontalLineValue = original[r][c].HorizontalLineValue,
                    VerticalLineValue = original[r][c].VerticalLineValue
                });
            }
            clone.Add(row);
        }
        return clone;
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
    
    private void GenerateHorizontalAndVerticalClues()
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
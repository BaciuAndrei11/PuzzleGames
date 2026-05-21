using PuzzleGames.Frontend.Models;

namespace PuzzleGames.Frontend.Utilities;

public static class TakuzuGameUtility
{
    private static readonly Random Random = new Random();

    public static bool GenerateFullRandomBoard(int row, int col, List<List<TakuzuCell>> takuzuBoard)
    {
        if (row == takuzuBoard.Count) 
            return true;
        
        int nextRow = col == takuzuBoard.Count - 1 ? row + 1 : row;
        int nextCol = col == takuzuBoard.Count - 1 ? 0 : col + 1;
        
        var options = Random.Next(2) == 0 
            ? new[] { TakuzuCellEnum.Fire, TakuzuCellEnum.Water } 
            : new[] { TakuzuCellEnum.Water, TakuzuCellEnum.Fire };
        
        foreach (var option in options)
        {
            takuzuBoard[row][col].Cell = option;

            if (IsValidPlacement(row, col, takuzuBoard))
            {
                if (GenerateFullRandomBoard(nextRow, nextCol, takuzuBoard)) return true; 
            }
        }
        
        takuzuBoard[row][col].Cell = TakuzuCellEnum.Empty;
        return false;
    }
    
    public static void GenerateHorizontalAndVerticalClues(List<List<TakuzuCell>> takuzuBoard)
    {
        for (int r = 0; r < takuzuBoard.Count; r++)
        {
            for (int c = 0; c < takuzuBoard.Count; c++)
            {
                if (c < takuzuBoard.Count - 1)
                    takuzuBoard[r][c].HorizontalLineValue = takuzuBoard[r][c].Cell == takuzuBoard[r][c + 1].Cell ? TakuzuLineEnum.Equal : TakuzuLineEnum.Different;
                
                if (r < takuzuBoard.Count - 1)
                    takuzuBoard[r][c].VerticalLineValue = takuzuBoard[r][c].Cell == takuzuBoard[r + 1][c].Cell ? TakuzuLineEnum.Equal : TakuzuLineEnum.Different;
            }
        }
    }
    
    public static bool IsBoardComplete(List<List<TakuzuCell>> board)
    {
        for (int r = 0; r < board.Count; r++)
        {
            for (int c = 0; c < board.Count; c++)
            {
                if (board[r][c].Cell == TakuzuCellEnum.Empty)
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    public static void GenerateClues(List<List<TakuzuCell>> takuzuBoard)
    {
        int rows = takuzuBoard.Count;
        int cols = takuzuBoard[0].Count;
        
        var allElements = new List<(ClueTypeEnum Type, int R, int C)>();

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                allElements.Add((ClueTypeEnum.Cell, r, c));

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols - 1; c++)
                allElements.Add((ClueTypeEnum.Horizontal, r, c));

        for (int r = 0; r < rows - 1; r++)
            for (int c = 0; c < cols; c++)
                allElements.Add((ClueTypeEnum.Vertical, r, c));

        allElements = allElements.OrderBy(x => Random.Next()).ToList();

        foreach (var element in allElements)
        {
            if (element.Type == ClueTypeEnum.Cell)
            {
                TakuzuCellEnum originalValue = takuzuBoard[element.R][element.C].Cell;
                takuzuBoard[element.R][element.C].Cell = TakuzuCellEnum.Empty;

                if (!CanBeSolvedLogically(CloneBoard(takuzuBoard)))
                {
                    takuzuBoard[element.R][element.C].Cell = originalValue;
                    takuzuBoard[element.R][element.C].IsFixed = true; 
                }
            }
            else if (element.Type == ClueTypeEnum.Horizontal)
            {
                TakuzuLineEnum originalLine = takuzuBoard[element.R][element.C].HorizontalLineValue;
                takuzuBoard[element.R][element.C].HorizontalLineValue = TakuzuLineEnum.None;

                if (!CanBeSolvedLogically(CloneBoard(takuzuBoard)))
                {
                    takuzuBoard[element.R][element.C].HorizontalLineValue = originalLine;
                }
            }
            else if (element.Type == ClueTypeEnum.Vertical)
            {
                TakuzuLineEnum originalLine = takuzuBoard[element.R][element.C].VerticalLineValue;
                takuzuBoard[element.R][element.C].VerticalLineValue = TakuzuLineEnum.None;

                if (!CanBeSolvedLogically(CloneBoard(takuzuBoard)))
                {
                    takuzuBoard[element.R][element.C].VerticalLineValue = originalLine;
                }
            }
        }

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (takuzuBoard[r][c].Cell != TakuzuCellEnum.Empty)
                    takuzuBoard[r][c].IsFixed = true;
    }
    
    private static bool CanBeSolvedLogically(List<List<TakuzuCell>> testBoard)
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
    
    private static bool ApplyConsecutiveRules(List<List<TakuzuCell>> board)
    {
        int rows = board.Count;
        int cols = board[0].Count;
        
        bool madeProgress = false;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (c <= cols - 3)
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

                if (r <= rows - 3)
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
    
    private static bool ApplyCountingRules(List<List<TakuzuCell>> board)
    {
        int rows = board.Count;
        int cols = board[0].Count;
        
        bool madeProgress = false;
        int maxPerLine = rows / 2;

        for (int r = 0; r < rows; r++)
        {
            int waterCount = 0; int fireCount = 0;
            for (int c = 0; c < cols; c++)
            {
                if (board[r][c].Cell == TakuzuCellEnum.Water) waterCount++;
                if (board[r][c].Cell == TakuzuCellEnum.Fire) fireCount++;
            }

            if (waterCount == maxPerLine && fireCount < maxPerLine)
            {
                for (int c = 0; c < cols; c++) if (board[r][c].Cell == TakuzuCellEnum.Empty) { board[r][c].Cell = TakuzuCellEnum.Fire; madeProgress = true; }
            }
            else if (fireCount == maxPerLine && waterCount < maxPerLine)
            {
                for (int c = 0; c < cols; c++) if (board[r][c].Cell == TakuzuCellEnum.Empty) { board[r][c].Cell = TakuzuCellEnum.Water; madeProgress = true; }
            }
        }

        for (int c = 0; c < cols; c++)
        {
            int waterCount = 0; int fireCount = 0;
            for (int r = 0; r < rows; r++)
            {
                if (board[r][c].Cell == TakuzuCellEnum.Water) waterCount++;
                if (board[r][c].Cell == TakuzuCellEnum.Fire) fireCount++;
            }

            if (waterCount == maxPerLine && fireCount < maxPerLine)
            {
                for (int r = 0; r < rows; r++) if (board[r][c].Cell == TakuzuCellEnum.Empty) { board[r][c].Cell = TakuzuCellEnum.Fire; madeProgress = true; }
            }
            else if (fireCount == maxPerLine && waterCount < maxPerLine)
            {
                for (int r = 0; r < rows; r++) if (board[r][c].Cell == TakuzuCellEnum.Empty) { board[r][c].Cell = TakuzuCellEnum.Water; madeProgress = true; }
            }
        }
        return madeProgress;
    }
    
    private static bool ApplyClueRules(List<List<TakuzuCell>> board)
    {
        int rows = board.Count;
        int cols = board[0].Count;
        
        bool madeProgress = false;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (c < cols - 1 && board[r][c].HorizontalLineValue != TakuzuLineEnum.None)
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

                if (r < rows - 1 && board[r][c].VerticalLineValue != TakuzuLineEnum.None)
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
    
    private static TakuzuCellEnum GetOpposite(TakuzuCellEnum cellType) =>
        cellType == TakuzuCellEnum.Water ? TakuzuCellEnum.Fire : TakuzuCellEnum.Water;

    private static List<List<TakuzuCell>> CloneBoard(List<List<TakuzuCell>> original)
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
    
    private static bool IsValidPlacement(int row, int col, List<List<TakuzuCell>> takuzuBoard)
    {
        TakuzuCellEnum currentType = takuzuBoard[row][col].Cell;

        if (col >= 2 && takuzuBoard[row][col - 1].Cell == currentType && takuzuBoard[row][col - 2].Cell == currentType) return false;
        if (col >= 1 && col < takuzuBoard.Count - 1 && takuzuBoard[row][col - 1].Cell == currentType && takuzuBoard[row][col + 1].Cell == currentType) return false;
        if (col < takuzuBoard.Count - 2 && takuzuBoard[row][col + 1].Cell == currentType && takuzuBoard[row][col + 2].Cell == currentType) return false;

        if (row >= 2 && takuzuBoard[row - 1][col].Cell == currentType && takuzuBoard[row - 2][col].Cell == currentType) return false;
        if (row >= 1 && row < takuzuBoard.Count - 1 && takuzuBoard[row - 1][col].Cell == currentType && takuzuBoard[row + 1][col].Cell == currentType) return false;
        if (row < takuzuBoard.Count - 2 && takuzuBoard[row + 1][col].Cell == currentType && takuzuBoard[row + 2][col].Cell == currentType) return false;

        int maxAllowed = takuzuBoard.Count / 2;
        
        int rowCount = 0;
        for (int c = 0; c < takuzuBoard.Count; c++)
        {
            if (takuzuBoard[row][c].Cell == currentType) 
                rowCount++;
        }
        if (rowCount > maxAllowed) 
            return false;

        int colCount = 0;
        for (int r = 0; r < takuzuBoard.Count; r++)
        {
            if (takuzuBoard[r][col].Cell == currentType) 
                colCount++;
        }

        if (colCount > maxAllowed) 
            return false;

        return true;
    }
}
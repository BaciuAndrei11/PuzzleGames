using PuzzleGames.Frontend.Models;

namespace PuzzleGames.Frontend.Utilities;

public static class StarBattleUtility
{
    private static readonly Random Random = new Random();
    
    public static bool PlaceStarsBacktracking(List<List<StarBattleCell>>board, List<(int R, int C)> starPositions, int row, int size)
    {
        if (row == size) return true;

        var columns = Enumerable.Range(0, size).OrderBy(_ => Random.Next()).ToList();

        foreach (var col in columns)
        {
            if (IsValidStarPosition(starPositions, row, col))
            {
                starPositions.Add((row, col));
                board[row][col].Cell = StarBattleCellEnum.Empty;

                if (PlaceStarsBacktracking(board, starPositions, row + 1, size))
                    return true;

                starPositions.RemoveAt(starPositions.Count - 1);
            }
        }

        return false;
    }

    public static List<List<StarBattleCell>> CloneBoard(List<List<StarBattleCell>> original)
    {
        var clone = new List<List<StarBattleCell>>();
        for (int r = 0; r < original.Count; r++)
        {
            var row = new List<StarBattleCell>();
            for (int c = 0; c < original[r].Count; c++)
            {
                row.Add(new StarBattleCell(original[r][c].Cell, original[r][c].Color));
            }
            clone.Add(row);
        }

        return clone;
    }

    private static bool IsValidStarPosition(List<(int R, int C)> starPositions, int row, int col)
    {
        foreach (var star in starPositions)
        {
            if (star.C == col) return false;
            if (Math.Abs(star.R - row) <= 1 && Math.Abs(star.C - col) <= 1) return false;
        }
        return true;
    }

    public static void ExpandRegions(List<List<StarBattleCell>> board, int size)
    {
        var unassignedCells = new List<(int R, int C)>();
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if ((int)board[r][c].Color == -1)
                    unassignedCells.Add((r, c));
            }
        }

        int[] dRow = { -1, 1, 0, 0 };
        int[] dCol = { 0, 0, -1, 1 };

        while (unassignedCells.Count > 0)
        {
            unassignedCells = unassignedCells.OrderBy(_ => Random.Next()).ToList();
            bool madeProgress = false;

            for (int i = unassignedCells.Count - 1; i >= 0; i--)
            {
                var (r, c) = unassignedCells[i];
                var neighboringColors = new List<StarBattleCellColorEnum>();

                for (int d = 0; d < 4; d++)
                {
                    int nRow = r + dRow[d];
                    int nCol = c + dCol[d];

                    if (nRow >= 0 && nRow < size && nCol >= 0 && nCol < size)
                    {
                        var neighborColor = board[nRow][nCol].Color;
                        if ((int)neighborColor != -1)
                        {
                            neighboringColors.Add(neighborColor);
                        }
                    }
                }

                if (neighboringColors.Count > 0)
                {
                    board[r][c].Color = neighboringColors[Random.Next(neighboringColors.Count)];
                    unassignedCells.RemoveAt(i);
                    madeProgress = true;
                }
            }

            if (!madeProgress && unassignedCells.Count > 0)
            {
                var (r, c) = unassignedCells[0];
                board[r][c].Color = (StarBattleCellColorEnum)Random.Next(size);
                unassignedCells.RemoveAt(0);
            }
        }
    }
    
    public static bool CanBeSolvedLogically(List<List<StarBattleCell>>board)
    {
        bool progressMade;
        do
        {
            progressMade = false;

            if (ApplyRegionRule(board)) progressMade = true;
            if (ApplyLineRules(board)) progressMade = true;
            if (ApplyAdvancedIntersectionRule(board)) progressMade = true;

        } while (progressMade);

        return CountTotalStars(board) == board.Count;
    }

    private static bool ApplyRegionRule(List<List<StarBattleCell>>board)
    {
        bool changed = false;

        for (int colorIdx = 0; colorIdx < board.Count; colorIdx++)
        {
            var regionCells = GetCellsByColor(board, (StarBattleCellColorEnum)colorIdx);
            
            if (regionCells.Any(c => board[c.R][c.C].Cell == StarBattleCellEnum.Star))
            {
                foreach (var c in regionCells.Where(c => board[c.R][c.C].Cell == StarBattleCellEnum.Empty))
                {
                    board[c.R][c.C].Cell = StarBattleCellEnum.MarkedX;
                    changed = true;
                }
                continue;
            }

            var emptyCells = regionCells.Where(c => board[c.R][c.C].Cell == StarBattleCellEnum.Empty).ToList();
            if (emptyCells.Count == 1)
            {
                PlaceStar(board, emptyCells[0].R, emptyCells[0].C);
                changed = true;
            }
        }

        return changed;
    }

    private static bool ApplyLineRules(List<List<StarBattleCell>>board)
    {
        bool changed = false;

        for (int i = 0; i < board.Count; i++)
        {
            var rowCells = Enumerable.Range(0, board.Count).Select(c => (R: i, C: c)).ToList();
            if (ProcessLine(board, rowCells)) changed = true;

            var colCells = Enumerable.Range(0, board.Count).Select(r => (R: r, C: i)).ToList();
            if (ProcessLine(board, colCells)) changed = true;
        }

        return changed;
    }

    private static bool ProcessLine(List<List<StarBattleCell>>board, List<(int R, int C)> line)
    {
        bool changed = false;
        
        if (line.Any(pos => board[pos.R][pos.C].Cell == StarBattleCellEnum.Star))
        {
            foreach (var pos in line.Where(pos => board[pos.R][pos.C].Cell == StarBattleCellEnum.Empty))
            {
                board[pos.R][pos.C].Cell = StarBattleCellEnum.MarkedX;
                changed = true;
            }
            return changed;
        }

        var empty = line.Where(pos => board[pos.R][pos.C].Cell == StarBattleCellEnum.Empty).ToList();
        if (empty.Count == 1)
        {
            PlaceStar(board, empty[0].R, empty[0].C);
            changed = true;
        }

        return changed;
    }

    private static bool ApplyAdvancedIntersectionRule(List<List<StarBattleCell>>board)
    {
        bool changed = false;

        for (int colorIdx = 0; colorIdx < board.Count; colorIdx++)
        {
            var regionCells = GetCellsByColor(board, (StarBattleCellColorEnum)colorIdx)
                .Where(pos => board[pos.R][pos.C].Cell == StarBattleCellEnum.Empty).ToList();

            if (regionCells.Count == 0) continue;

            int firstRow = regionCells[0].R;
            if (regionCells.All(pos => pos.R == firstRow))
            {
                for (int c = 0; c < board.Count; c++)
                {
                    if (board[firstRow][c].Cell == StarBattleCellEnum.Empty && board[firstRow][c].Color != (StarBattleCellColorEnum)colorIdx)
                    {
                        board[firstRow][c].Cell = StarBattleCellEnum.MarkedX;
                        changed = true;
                    }
                }
            }

            int firstCol = regionCells[0].C;
            if (regionCells.All(pos => pos.C == firstCol))
            {
                for (int r = 0; r < board.Count; r++)
                {
                    if (board[r][firstCol].Cell == StarBattleCellEnum.Empty && board[r][firstCol].Color != (StarBattleCellColorEnum)colorIdx)
                    {
                        board[r][firstCol].Cell = StarBattleCellEnum.MarkedX;
                        changed = true;
                    }
                }
            }
        }

        return changed;
    }

    public static bool IsGameOver(List<List<StarBattleCell>> board)
    {
        int size = board.Count;

        int totalStarsOnBoard = CountTotalStars(board);

        if (totalStarsOnBoard != size)
        {
            return false;
        }

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (board[r][c].Cell == StarBattleCellEnum.Star)
                {
                    if (HasConflicts(board, r, c))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
    
    private static bool HasConflicts(List<List<StarBattleCell>> board, int row, int col)
    {
        int size = board.Count;

        for (int i = 0; i < size; i++)
        {
            if (i != col && board[row][i].Cell == StarBattleCellEnum.Star) return true;
        
            if (i != row && board[i][col].Cell == StarBattleCellEnum.Star) return true;
        }

        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = col - 1; c <= col + 1; c++)
            {
                if (r >= 0 && r < size && c >= 0 && c < size && (r != row || c != col))
                {
                    if (board[r][c].Cell == StarBattleCellEnum.Star) return true;
                }
            }
        }

        var currentColor = board[row][col].Color;
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if ((r != row || c != col) && board[r][c].Color == currentColor && board[r][c].Cell == StarBattleCellEnum.Star)
                {
                    return true;
                }
            }
        }

        return false; 
    }
    
    public static void ValidateBoard(List<List<StarBattleCell>> board)
    {
        int size = board.Count;

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                board[r][c].IsValid = true;
            }
        }

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                if (board[r][c].Cell == StarBattleCellEnum.Star && HasConflicts(board, r, c))
                {
                    if (board[r].Count(cell => cell.Cell == StarBattleCellEnum.Star) > 1)
                    {
                        for (int i = 0; i < size; i++) board[r][i].IsValid = false;
                    }
                    
                    int starsInCol = 0;
                    for (int i = 0; i < size; i++) if (board[i][c].Cell == StarBattleCellEnum.Star) starsInCol++;
                        if (starsInCol > 1)
                        {
                            for (int i = 0; i < size; i++) board[i][c].IsValid = false;
                        }
                        
                    var currentColor = board[r][c].Color;
                    int starsInRegion = 0;
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            if (board[i][j].Color == currentColor && board[i][j].Cell == StarBattleCellEnum.Star)
                                starsInRegion++;
                        }
                    }
                    if (starsInRegion > 1)
                    {
                        for (int i = 0; i < size; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                if (board[i][j].Color == currentColor) board[i][j].IsValid = false;
                            }
                        }
                    }

                    for (int nR = r - 1; nR <= r + 1; nR++)
                    {
                        for (int nC = c - 1; nC <= c + 1; nC++)
                        {
                            if (nR >= 0 && nR < size && nC >= 0 && nC < size && (nR != r || nC != c))
                            {
                                if (board[nR][nC].Cell == StarBattleCellEnum.Star)
                                {
                                    board[r][c].IsValid = false;
                                    board[nR][nC].IsValid = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    public static void PlaceStar(List<List<StarBattleCell>>board, int row, int col)
    {
        board[row][col].Cell = StarBattleCellEnum.Star;
        
        for (int i = 0; i < board.Count; i++)
        {
            if (i != col && board[row][i].Cell == StarBattleCellEnum.Empty)
            {
                board[row][i].Cell = StarBattleCellEnum.MarkedX;
            }

            if (i != row && board[i][col].Cell == StarBattleCellEnum.Empty)
            {
                board[i][col].Cell = StarBattleCellEnum.MarkedX;
            }
        }

        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = col - 1; c <= col + 1; c++)
            {
                if (r >= 0 && r < board.Count && c >= 0 && c < board.Count && (r != row || c != col))
                {
                    if (board[r][c].Cell == StarBattleCellEnum.Empty)
                    {
                        board[r][c].Cell = StarBattleCellEnum.MarkedX;
                    }
                }
            }
        }
    }

    public static void ClearStar(List<List<StarBattleCell>> board, int row, int col)
    {
        for (int i = 0; i < board.Count; i++)
        {
            if (i != col && board[row][i].Cell == StarBattleCellEnum.MarkedX && board[row][i].IsPlacedByUser == false)
            {
                board[row][i].Cell = StarBattleCellEnum.Empty;
            }
            if (i != row && board[i][col].Cell == StarBattleCellEnum.MarkedX && board[i][col].IsPlacedByUser == false)
            {
                board[i][col].Cell = StarBattleCellEnum.Empty;
            }
        }
        
        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = col - 1; c <= col + 1; c++)
            {
                if (r >= 0 && r < board.Count && c >= 0 && c < board.Count && (r != row || c != col) && board[r][c].IsPlacedByUser == false)
                {
                    if (board[r][c].Cell == StarBattleCellEnum.MarkedX)
                    {
                        board[r][c].Cell = StarBattleCellEnum.Empty;
                    }
                }
            }
        }
        
    }

    private static List<(int R, int C)> GetCellsByColor(List<List<StarBattleCell>>board, StarBattleCellColorEnum color)
    {
        var cells = new List<(int R, int C)>();
        for (int r = 0; r < board.Count; r++)
        {
            for (int c = 0; c < board.Count; c++)
            {
                if (board[r][c].Color == color) cells.Add((r, c));
            }
        }
        return cells;
    }

    private static int CountTotalStars(List<List<StarBattleCell>>board)
    {
        return board.Sum(row => row.Count(c => c.Cell == StarBattleCellEnum.Star));
    }
    
}
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
}
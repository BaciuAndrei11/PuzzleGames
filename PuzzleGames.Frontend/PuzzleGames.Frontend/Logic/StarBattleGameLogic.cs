using PuzzleGames.Frontend.Models;
using PuzzleGames.Frontend.Utilities;

namespace PuzzleGames.Frontend.Logic;

public class StarBattleGameLogic :  IGameLogic
{
    public List<List<StarBattleCell>> StarBattleBoard { get; set; }
    public bool IsGameOver { get; set; }
    

    public void GenerateNewGame(int size)
    {
        IsGameOver = false;
        bool isValidBoard = false;
        while (isValidBoard == false)
        {
            StarBattleBoard = new List<List<StarBattleCell>>();
            for (int r = 0; r < size; r++)
            {
                var row = new List<StarBattleCell>();
                for (int c = 0; c < size; c++)
                {
                    row.Add(new StarBattleCell(StarBattleCellEnum.Empty, (StarBattleCellColorEnum)(-1)));
                }

                StarBattleBoard.Add(row);
            }

            List<(int R, int C)> starPositions = new List<(int R, int C)>();
            if (!StarBattleUtility.PlaceStarsBacktracking(StarBattleBoard, starPositions, 0, size))
            {
                GenerateNewGame(size);
            }

            for (int i = 0; i < starPositions.Count; i++)
            {
                var pos = starPositions[i];
                StarBattleBoard[pos.R][pos.C].Color = (StarBattleCellColorEnum)i;
            }

            StarBattleUtility.ExpandRegions(StarBattleBoard, size);
            isValidBoard = StarBattleUtility.CanBeSolvedLogically(StarBattleUtility.CloneBoard(StarBattleBoard));
        }
    }

    public int GetBoardSize()
    {
        return StarBattleBoard.Count;
    }
    
    public string GetFormattedTime()
    {
        return $"";
    }

    public async Task ChangeCellValueAsync(int row, int col)
    {
        switch (StarBattleBoard[row][col].Cell) 
        {   
            case StarBattleCellEnum.Empty:
                StarBattleBoard[row][col].Cell = StarBattleCellEnum.MarkedX;
                break;
            case StarBattleCellEnum.MarkedX:
                StarBattleBoard[row][col].Cell = StarBattleCellEnum.Star;
                break;
            case StarBattleCellEnum.Star:
                StarBattleBoard[row][col].Cell = StarBattleCellEnum.Empty;
                break;
        }
        IsGameOver = StarBattleUtility.IsGameOver(StarBattleBoard);
    }
}


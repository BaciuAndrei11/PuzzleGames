using PuzzleGames.Frontend.Models;
using PuzzleGames.Frontend.Utilities;

namespace PuzzleGames.Frontend.Logic;

public class StarBattleGameLogic :  IGameLogic
{
    public List<List<StarBattleCell>> StarBattleBoard { get; set; }
    public bool IsGameOver { get; set; }
    
    private System.Timers.Timer? _timer;
    private int _secondsElapsed = 0;
    
    public event Action? OnTimerTicked;

    public string FormattedTime => 
        $"{( _secondsElapsed / 60 ):D2}:{( _secondsElapsed % 60 ):D2}";

    public string GetFormattedTime()
    {
        return FormattedTime;
    }
    
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
            
            StartTimer();
        }
    }

    public int GetBoardSize()
    {
        return StarBattleBoard.Count;
    }

    public async Task ChangeCellValueAsync(int row, int col)
    {
        switch (StarBattleBoard[row][col].Cell) 
        {   
            case StarBattleCellEnum.Empty:
                StarBattleBoard[row][col].Cell = StarBattleCellEnum.MarkedX;
                StarBattleBoard[row][col].IsPlecedByUser = true;
                break;
            case StarBattleCellEnum.MarkedX:
                StarBattleBoard[row][col].Cell = StarBattleCellEnum.Star;
                StarBattleUtility.PlaceStar(StarBattleBoard, row, col);
                StarBattleBoard[row][col].IsPlecedByUser = true;
                break; 
            case StarBattleCellEnum.Star:
                StarBattleBoard[row][col].Cell = StarBattleCellEnum.Empty;
                StarBattleUtility.ClearStar(StarBattleBoard, row, col);
                StarBattleBoard[row][col].IsPlecedByUser = false;
                break;
        }
        IsGameOver = StarBattleUtility.IsGameOver(StarBattleBoard);
        if (IsGameOver)
        {
            StopTimer();
        }
    }
    
    public void StartTimer()
    {
        _timer?.Stop();
        _timer?.Dispose();

        _secondsElapsed = 0;
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (sender, e) =>
        {
            if (IsGameOver)
            {
                _timer?.Stop();
            }
            else
            {
                _secondsElapsed++;
                OnTimerTicked?.Invoke();
            }
        };
        _timer.AutoReset = true;
        _timer.Start();
    }

    public void StopTimer() => _timer?.Stop();
}


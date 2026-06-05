using PuzzleGames.Frontend.Models;
using PuzzleGames.Frontend.Utilities;

namespace PuzzleGames.Frontend.Logic;

public class StarBattleGameLogic :  IGameLogic
{
    public List<List<StarBattleCell>> StarBattleBoard { get; set; }
    public List<List<StarBattleCell>> GeneratedBoard { get; set; }
    public bool IsGameOver { get; set; }
    
    private Stack<MoveAction> _undoStack = new Stack<MoveAction>();
    private Stack<MoveAction> _redoStack = new Stack<MoveAction>();
    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    
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
        _undoStack.Clear();
        _redoStack.Clear();
        
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
            
            GeneratedBoard = StarBattleUtility.CloneBoard(StarBattleBoard);
            
            StartTimer();
        }
    }

    public int GetBoardSize()
    {
        return StarBattleBoard.Count;
    }
    
    public void ResetBoard()
    {
        StarBattleBoard = StarBattleUtility.CloneBoard(GeneratedBoard);
        _undoStack.Clear();
        _redoStack.Clear();
    }

    public async Task ChangeCellValueAsync(int row, int col)
    {
        var previousValue = StarBattleBoard[row][col].Cell;

        switch (StarBattleBoard[row][col].Cell) 
        {   
            case StarBattleCellEnum.Empty:
                StarBattleBoard[row][col].Cell = StarBattleCellEnum.MarkedX;
                StarBattleBoard[row][col].IsPlacedByUser = true;
                break;
            case StarBattleCellEnum.MarkedX:
                StarBattleBoard[row][col].Cell = StarBattleCellEnum.Star;
                StarBattleUtility.PlaceStar(StarBattleBoard, row, col);
                StarBattleBoard[row][col].IsPlacedByUser = true;
                break; 
            case StarBattleCellEnum.Star:
                StarBattleBoard[row][col].Cell = StarBattleCellEnum.Empty;
                StarBattleUtility.ClearStar(StarBattleBoard, row, col);
                StarBattleBoard[row][col].IsPlacedByUser = false;
                break;
        }
        StarBattleUtility.ValidateBoard(StarBattleBoard);
        IsGameOver = StarBattleUtility.IsGameOver(StarBattleBoard);
        if (IsGameOver)
        {
            StopTimer();
        }
        
        var newValue = StarBattleBoard[row][col].Cell;
        _undoStack.Push(new MoveAction
        {
            Row = row,
            Col = col,
            PreviousValue = (int)previousValue,
            NewValue = (int)newValue
        });

        _redoStack.Clear();
    }
    
    public void Undo()
    {
        if (!CanUndo) 
            return;

        var lastMove = _undoStack.Pop();
        _redoStack.Push(lastMove);

        StarBattleBoard[lastMove.Row][lastMove.Col].Cell = (StarBattleCellEnum)lastMove.PreviousValue;
        if (StarBattleBoard[lastMove.Row][lastMove.Col].Cell == StarBattleCellEnum.Star)
        {
            StarBattleUtility.PlaceStar(StarBattleBoard, lastMove.Row, lastMove.Col);
        }
        else
        {
            StarBattleUtility.ClearStar(StarBattleBoard, lastMove.Row, lastMove.Col);
        }
        StarBattleUtility.ValidateBoard(StarBattleBoard);
    }

    public void Redo()
    {
        if (!CanRedo) 
            return;

        var nextMove = _redoStack.Pop();
        _undoStack.Push(nextMove);

        StarBattleBoard[nextMove.Row][nextMove.Col].Cell = (StarBattleCellEnum)nextMove.NewValue;
        if (StarBattleBoard[nextMove.Row][nextMove.Col].Cell == StarBattleCellEnum.Star)
        {
            StarBattleUtility.PlaceStar(StarBattleBoard, nextMove.Row, nextMove.Col);
        }
        else
        {
            StarBattleUtility.ClearStar(StarBattleBoard, nextMove.Row, nextMove.Col);
        }
        StarBattleUtility.ValidateBoard(StarBattleBoard);
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


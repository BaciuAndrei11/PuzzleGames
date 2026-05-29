using PuzzleGames.Frontend.Clients;
using PuzzleGames.Frontend.Models;
using PuzzleGames.Frontend.Utilities;

namespace PuzzleGames.Frontend.Logic;

public class TakuzuGameLogic
{
    private readonly UserSession _userSession;
    private readonly UserClient _userClient;

    // Sistemul de Dependency Injection va aduce AUTOMAT sesiunea curentă aici
    public TakuzuGameLogic(UserSession userSession, UserClient userClient)
    {
        _userSession = userSession;
        _userClient = userClient;
    }
    public List<List<TakuzuCell>> TakuzuBoard { get; set; }
    public List<List<TakuzuCell>> GeneratedBoard { get; set; }
    public int Rows { get; set; }
    public int Cols { get; set; }
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

    public void GenerateNewGame(int size)
    {
        _undoStack.Clear();
        _redoStack.Clear();
        
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
        StartTimer();
    }

    public async Task ChangeCellValueAsync(int row, int col)
    {
        if(TakuzuBoard[row][col].IsFixed == true)
            return;
        
        var previousValue = TakuzuBoard[row][col].Cell;
        
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
        if (IsGameOver)
        {
            if (_userSession.IsLoggedIn)
            {
                _userSession.CurrentUser.CurrentLevel = _userSession.CurrentUser.CurrentLevel + 1;
                await _userClient.UpdateUserAsync(_userSession.CurrentUser);
                
                _userSession.NotifyStateChanged();
            }

            StopTimer();
        }
        
        var newValue = TakuzuBoard[row][col].Cell;
        _undoStack.Push(new MoveAction
        {
            Row = row,
            Col = col,
            PreviousValue = previousValue,
            NewValue = newValue
        });

        _redoStack.Clear();
    }

    public void ResetBoard()
    {
        TakuzuBoard = TakuzuGameUtility.CloneBoard(GeneratedBoard);
        _undoStack.Clear();
        _redoStack.Clear();
    }
    
    public void Undo()
    {
        if (!CanUndo) 
            return;

        var lastMove = _undoStack.Pop();
        _redoStack.Push(lastMove);

        TakuzuBoard[lastMove.Row][lastMove.Col].Cell = lastMove.PreviousValue;

        TakuzuGameUtility.ValidateBoard(TakuzuBoard);
    }

    public void Redo()
    {
        if (!CanRedo) 
            return;

        var nextMove = _redoStack.Pop();
        _undoStack.Push(nextMove);

        TakuzuBoard[nextMove.Row][nextMove.Col].Cell = nextMove.NewValue;

        TakuzuGameUtility.ValidateBoard(TakuzuBoard);
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
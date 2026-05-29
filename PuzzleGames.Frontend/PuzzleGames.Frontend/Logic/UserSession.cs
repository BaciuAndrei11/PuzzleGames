using PuzzleGames.Frontend.Models;

namespace PuzzleGames.Frontend.Logic;

public class UserSession
{
    public User? CurrentUser { get; set; }
    
    public bool IsLoggedIn => CurrentUser != null;

    public event Action? OnChange;
    public void NotifyStateChanged() => OnChange?.Invoke();

    public async Task LogOutAsync()
    {
        CurrentUser = null;
    }
}
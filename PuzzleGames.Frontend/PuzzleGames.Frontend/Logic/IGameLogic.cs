using PuzzleGames.Frontend.Models;

namespace PuzzleGames.Frontend.Logic;

public interface IGameLogic
{
    public void GenerateNewGame(int size);
    public int GetBoardSize();
    public string GetFormattedTime();
}
using BCryptNet;
using PuzzleGames.Frontend.Clients;
using PuzzleGames.Frontend.Models;

namespace PuzzleGames.Frontend.Logic;

public class RegisterLogic
{
    private UserSession _userSesion;
    private UserClient _userClient;

    public RegisterLogic(UserSession userSession, UserClient userClient)
    {
        _userSesion = userSession;
        _userClient = userClient;
    }
    public async Task RegisterUserAsync(RegisterUser registerUser)
    {
        User newUser = new User()
        {
            Username = registerUser.Username,
            Password = registerUser.Password,
            Email = registerUser.Email,
            CurrentLevel = 1
        };
        await _userClient.AddUserAsync(newUser);
        _userSesion.CurrentUser = newUser;
    }
}
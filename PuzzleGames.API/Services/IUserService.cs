using PuzzleGames.API.Dtos;
using PuzzleGames.API.Entities;

namespace PuzzleGames.API.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> GetUserAsync(int id);
    Task<UserDto> GetUserByUsernameAsync(string username, string password);
    Task<UserDto> CreateUserAsync(CreateUserDto newUser);
    Task UpdateUserAsync(int id, UpdateUserDto updatedUser);
    Task DeleteUserAsync(int id);
}
using Microsoft.EntityFrameworkCore;
using PuzzleGames.API.Data;
using PuzzleGames.API.Dtos;
using PuzzleGames.API.Entities;
using PuzzleGames.API.Mapping;

namespace PuzzleGames.API.Services;

public class UserService(PuzzleGamesContext dbContext) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        return await dbContext.Users.Select(user => user.ToDto())
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        var user = await dbContext.Users.FindAsync(id);
        return user?.ToDto();
    }

    public async Task<UserDto> GetUserByUsernameAsync(string username, string password)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username 
                                      && u.Password == password);
        return user?.ToDto();
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto newUser)
    {
        User user = newUser.ToEntity();
            
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user.ToDto();
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto updatedUser)
    {
        var existingUser = await dbContext.Users.FindAsync(id);
        if (existingUser is not null)
        {
            dbContext.Entry(existingUser).CurrentValues.SetValues(updatedUser.ToEntity(id));
            await dbContext.SaveChangesAsync();
        }
       
    }

    public async Task DeleteUserAsync(int id)
    {
        await dbContext.Users.Where(user => user.Id == id).ExecuteDeleteAsync();
    }
}
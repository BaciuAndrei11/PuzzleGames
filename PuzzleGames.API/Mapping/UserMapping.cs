using BCryptNet;
using PuzzleGames.API.Dtos;
using PuzzleGames.API.Entities;

namespace PuzzleGames.API.Mapping;

public static class UserMapping
{
    public static User ToEntity(this CreateUserDto userDto)
    {
        return new User()
        {
            Username = userDto.Username,
            Email = userDto.Email,
            Password = BCrypt.HashPassword(userDto.Password),
        };
    }
    
    public static User ToEntity(this UpdateUserDto userDto, int id)
    {
        return new User()
        {
            Id = id,
            Username = userDto.Username,
            Email = userDto.Email,
            Password = userDto.Password,
        };
    }

    public static UserDto ToDto(this User user)
    {
        return new  UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.Password,
            user.GameProgresses.Select(p => new GamesProgressDto(
                p.Id,
                p.GameType,
                p.CurrentLevel))
        );
    }
}
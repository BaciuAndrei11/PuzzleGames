using System.ComponentModel.DataAnnotations;

namespace PuzzleGames.API.Dtos;

public record CreateUserDto(
    [Required]string Username,
    [Required]string Password,
    [Required]string Email
    );
using System.ComponentModel.DataAnnotations;

namespace PuzzleGames.API.Dtos;

public record UpdateUserDto(
    [Required]string Username,
    [Required]string Password,
    [Required]string Email,
    [Required]int CurrentLevel
    );
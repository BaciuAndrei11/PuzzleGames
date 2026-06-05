using System.ComponentModel.DataAnnotations;
using PuzzleGames.API.Enums;

namespace PuzzleGames.API.Dtos;

public record UpdateProgressDto(
    [Required] GameType GameType,
    [Required] int NewLevel
    );
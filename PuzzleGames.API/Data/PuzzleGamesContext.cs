using Microsoft.EntityFrameworkCore;
using PuzzleGames.API.Entities;

namespace PuzzleGames.API.Data;

public class PuzzleGamesContext(DbContextOptions<PuzzleGamesContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}
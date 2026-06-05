using Microsoft.EntityFrameworkCore;
using PuzzleGames.API.Entities;

namespace PuzzleGames.API.Data;

public class PuzzleGamesContext(DbContextOptions<PuzzleGamesContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserGameProgress> UserGameProgresses => Set<UserGameProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserGameProgress>()
            .Property(p => p.GameType)
            .HasConversion<string>();
    }
}
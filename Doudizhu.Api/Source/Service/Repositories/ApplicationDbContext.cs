using System.Text.Json;
using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.Repositories;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<GameUser> GameUsers { get; set; }
    public DbSet<GameRecord> Records { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
                    .HasOne(g => g.CurrentUser)           // Define the navigation property
                    .WithMany()                           // Specify the inverse relationship (if applicable)
                    .HasForeignKey(g => g.CurrentUserId); // Define the foreign key

        modelBuilder.Entity<Game>()
                    .HasOne(g => g.LastUser)
                    .WithMany()
                    .HasForeignKey(g => g.LastUserId);
        
        modelBuilder.Entity<Game>()
                    .Property(t => t.ReservedCards)
                    .HasConversion(
                        t => JsonSerializer.Serialize(t, JsonSerializerOptions.Default),
                        t => JsonSerializer.Deserialize<List<Card>>(t, JsonSerializerOptions.Default) ?? new());

        modelBuilder.Entity<Game>()
                    .Property(t => t.LastCardSentence)
                    .HasConversion(
                        t => JsonSerializer.Serialize(t, JsonSerializerOptions.Default),
                        t => JsonSerializer.Deserialize<CardSentence>(t, JsonSerializerOptions.Default));

        
        modelBuilder.Entity<GameUser>()
                    .Property(t => t.Cards)
                    .HasConversion(
                        t => JsonSerializer.Serialize(t, JsonSerializerOptions.Default),
                        t => JsonSerializer.Deserialize<List<Card>>(t, JsonSerializerOptions.Default) ?? new());
        
        modelBuilder.Entity<GameRecord>()
                    .Property(t=>t.CardSentence)
                    .HasConversion(
                        t => JsonSerializer.Serialize(t, JsonSerializerOptions.Default),
                        t => JsonSerializer.Deserialize<CardSentence>(t, JsonSerializerOptions.Default));
            
    }
}
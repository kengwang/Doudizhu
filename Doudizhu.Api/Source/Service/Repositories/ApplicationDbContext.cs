using Doudizhu.Api.Models;
using Doudizhu.Api.Models.GameLogic;
using Microsoft.EntityFrameworkCore;

namespace Doudizhu.Api.Service.Repositories;

public class ApplicationDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<GameUser> GameUsers { get; set; }
}
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class GameStoreDbContext : DbContext
    {
        public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<GamePlatform> GamePlatforms { get; set; }
        public DbSet<GameGenre> GameGenres { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .Property(g => g.Price)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<GamePlatform>()
                .HasKey(gp => new { gp.GamesKey, gp.PlatformsId });

            modelBuilder.Entity<GamePlatform>()
                .HasOne(gp => gp.Game)
                .WithMany(g => g.GamePlatforms)
                .HasForeignKey(gp => gp.GamesKey);

            modelBuilder.Entity<GamePlatform>()
                .HasOne(gp => gp.Platform)
                .WithMany(p => p.GamePlatforms)
                .HasForeignKey(gp => gp.PlatformsId);

            modelBuilder.Entity<GameGenre>().HasKey(gg => new { gg.GamesKey, gg.GenresId });

            modelBuilder.Entity<GameGenre>()
                .HasOne(gg => gg.Game)
                .WithMany(g => g.GameGenres)
                .HasForeignKey(gg => gg.GamesKey);

            modelBuilder.Entity<GameGenre>()
                .HasOne(gg => gg.Genre)
                .WithMany(g => g.GameGenres)
                .HasForeignKey(gg => gg.GenresId);
        }
    }
}

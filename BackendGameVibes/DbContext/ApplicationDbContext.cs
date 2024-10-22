using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using BackendGameVibes.Models.Forum;

namespace BackendGameVibes.Data {
    public class ApplicationDbContext : IdentityDbContext<UserGameVibes> {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }

        public DbSet<UserGameVibes> Users {
            get; set;
        }
        public DbSet<ForumRole> ForumRoles {
            get; set;
        }
        //public DbSet<Role> Roles {  // IdentityRole
        //    get; set;
        //}
        public DbSet<Game> Games {
            get; set;
        }
        public DbSet<Platform> Platforms {
            get; set;
        }
        public DbSet<Genre> Genres {
            get; set;
        }
        public DbSet<GameImage> GameImages {
            get; set;
        }
        public DbSet<SystemRequirement> SystemRequirements {
            get; set;
        }
        public DbSet<Review> Reviews {
            get; set;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.LogTo(message => Debug.WriteLine(message));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // UserGameVibes entity
            modelBuilder.Entity<UserGameVibes>(entity => {
                entity.HasOne(u => u.ForumRole)
                    .WithMany(fr => fr.Users)
                    .HasForeignKey(u => u.ForumRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(u => u.Description)
                    .HasMaxLength(500);

                entity.Property(u => u.ExperiencePoints)
                    .HasDefaultValue(10);

                entity.Property(u => u.ForumRoleId)
                    .IsRequired(true)
                    .HasDefaultValue(1);

                entity
                    .HasMany(u => u.UserReviews)
                    .WithOne(r => r.UserGameVibes);

                entity
                    .HasMany(u => u.FollowedGames)
                    .WithMany(g => g.PlayersFollowing)
                    .UsingEntity(j => j.ToTable("UsersGamesFollow"));
            });

            // ForumRole entity
            modelBuilder.Entity<ForumRole>(entity => {
                entity.HasKey(fr => fr.Id);

                entity.Property(fr => fr.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(fr => fr.Threshold)
                    .IsRequired();

                entity.HasMany(fr => fr.Users)
                    .WithOne(u => u.ForumRole)
                    .HasForeignKey(u => u.ForumRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasData(
                    new ForumRole() { Id = 1, Name = "beginner", Threshold = 0 },
                    new ForumRole() { Id = 2, Name = "experienced", Threshold = 100 },
                    new ForumRole() { Id = 3, Name = "powerful", Threshold = 1000 },
                    new ForumRole() { Id = 4, Name = "superhero", Threshold = 10000 });
            });

            // Game entity
            modelBuilder.Entity<Game>(entity => {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(g => g.Description)
                    .HasMaxLength(1000);

                entity.HasMany(g => g.Platforms)
                    .WithMany(p => p.Games)
                    .UsingEntity(j => j.ToTable("GamesPlatforms")); // many to many so we need 3 table to store the relationship

                entity.HasMany(g => g.Genres)
                    .WithMany(gen => gen.Games)
                    .UsingEntity(j => j.ToTable("GamesGenres")); // many to many so we need 3 table to store the relationship

                entity.HasMany(g => g.GameImages)
                    .WithOne(img => img.Game)
                    .HasForeignKey(img => img.GameId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(g => g.SystemRequirements)
                    .WithOne(sr => sr.Game)
                    .HasForeignKey(sr => sr.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Platform entity
            modelBuilder.Entity<Platform>(entity => {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new Platform() { Id = 1, Name = "Windows" },
                    new Platform() { Id = 2, Name = "Linux" },
                    new Platform() { Id = 3, Name = "MacOS" });
            });

            // Genre entity
            modelBuilder.Entity<Genre>(entity => {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                // steam Ids with their corresponding name genres, jednak tego jest duzo wiecej więc z automatu będą się dodawać niewystępujące jeszcze w bazie
            });

            // GameImage entity
            modelBuilder.Entity<GameImage>(entity => {
                entity.HasKey(gi => gi.Id);

                entity.Property(gi => gi.ImagePath)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(gi => gi.Game)
                    .WithMany(g => g.GameImages)
                    .HasForeignKey(gi => gi.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // SystemRequirement entity
            modelBuilder.Entity<SystemRequirement>(entity => {
                entity.HasKey(sr => sr.Id);

                entity.Property(sr => sr.CpuRequirement)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(sr => sr.GpuRequirement)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(sr => sr.RamRequirement)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(sr => sr.DiskRequirement)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(sr => sr.OperatingSystemRequirement)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(sr => sr.Game)
                    .WithMany(g => g.SystemRequirements)
                    .HasForeignKey(sr => sr.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Review entity
            modelBuilder.Entity<Review>(entity => {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.GeneralScore)
                    .IsRequired()
                    .HasPrecision(3, 2);

                entity.Property(r => r.GraphicsScore)
                    .IsRequired()
                    .HasPrecision(3, 2);

                entity.Property(r => r.AudioScore)
                    .IsRequired()
                    .HasPrecision(3, 2);

                entity.Property(r => r.GameplayScore)
                    .IsRequired()
                    .HasPrecision(3, 2);

                entity.Property(r => r.Comment)
                    .HasMaxLength(1000);

                entity.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .IsRequired();

                entity.HasOne(r => r.UserGameVibes)
                    .WithMany(u => u.UserReviews)
                    .HasForeignKey(r => r.UserGameVibesId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Game)
                    .WithMany(g => g.Reviews)
                    .HasForeignKey(r => r.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}

using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

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
        public DbSet<Role> Roles {
            get; set;
        }
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
            //base.OnConfiguring(optionsBuilder);
            optionsBuilder.LogTo(message => Debug.WriteLine(message));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // UserGameVibes entity
            modelBuilder.Entity<UserGameVibes>(entity => {
                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.ForumRole)
                    .WithMany(fr => fr.Users)
                    .HasForeignKey(u => u.ForumRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(u => u.Description)
                    .HasMaxLength(500);

                entity.Property(u => u.ExperiencePoints)
                    .HasDefaultValue(0);

                entity.Property(u => u.RoleId)
                    .IsRequired(true)
                    .HasDefaultValue(1);

                entity.Property(u => u.ForumRoleId)
                    .IsRequired(true)
                    .HasDefaultValue(1);
            });

            // Role entity
            modelBuilder.Entity<Role>(entity => {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasMany(r => r.Users)
                    .WithOne(u => u.Role)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasData(
                    new Role() { Id = 1, Name = "Guest" },
                    new Role() { Id = 2, Name = "User" },
                    new Role() { Id = 3, Name = "Mod" },
                    new Role() { Id = 4, Name = "Admin" });
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

                entity.HasOne(g => g.Platform)
                    .WithMany(p => p.Games)
                    .HasForeignKey(g => g.PlatformId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(g => g.Genres)
                    .WithMany(gen => gen.Games)
                    .UsingEntity(j => j.ToTable("GameGenres"));

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
                    new Platform() { Id = 0, Name = "Windows" },
                    new Platform() { Id = 1, Name = "Linux" },
                    new Platform() { Id = 2, Name = "MacOS" });
            });

            // Genre entity
            modelBuilder.Entity<Genre>(entity => {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(
                    new Genre { Id = 1, Name = "Action" },
                    new Genre { Id = 2, Name = "Adventure" },
                    new Genre { Id = 3, Name = "RPG" },
                    new Genre { Id = 4, Name = "Shooter" },
                    new Genre { Id = 5, Name = "Strategy" },
                    new Genre { Id = 6, Name = "Sports" },
                    new Genre { Id = 7, Name = "Puzzle" },
                    new Genre { Id = 8, Name = "Simulation" },
                    new Genre { Id = 9, Name = "Horror" },
                    new Genre { Id = 10, Name = "Platformer" },
                    new Genre { Id = 11, Name = "Racing" },
                    new Genre { Id = 12, Name = "Fighting" },
                    new Genre { Id = 13, Name = "MMO" },
                    new Genre { Id = 14, Name = "Survival" },
                    new Genre { Id = 15, Name = "Music" },
                    new Genre { Id = 16, Name = "Stealth" },
                    new Genre { Id = 17, Name = "Party" },
                    new Genre { Id = 18, Name = "Arcade" },
                    new Genre { Id = 19, Name = "Sandbox" },
                    new Genre { Id = 20, Name = "Card Game" }
                );
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
                    .IsRequired();

                entity.HasOne(r => r.UserGameVibes)
                    .WithMany()
                    .HasForeignKey(r => r.UserGameVibesId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Game)
                    .WithMany()
                    .HasForeignKey(r => r.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}

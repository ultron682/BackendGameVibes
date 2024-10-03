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
                    new Role() { Id = 1, Name = "guest" },
                    new Role() { Id = 2, Name = "user" },
                    new Role() { Id = 3, Name = "mod" },
                    new Role() { Id = 4, Name = "admin" });
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
                //entity.HasData(
                //    new Genre { Id = 23304, Name = "Puzzle" },
                //    new Genre { Id = 16702, Name = "Action-Adventure" },
                //    new Genre { Id = 16413, Name = "Arcade" },
                //    new Genre { Id = 14558, Name = "Shooter" },
                //    new Genre { Id = 12501, Name = "Platformer" },
                //    new Genre { Id = 11002, Name = "Visual Novel" },
                //    new Genre { Id = 8128, Name = "Sandbox" },
                //    new Genre { Id = 7914, Name = "Action RPG" },
                //    new Genre { Id = 7766, Name = "Rogue-like" },
                //    new Genre { Id = 7620, Name = "Point & Click" },
                //    new Genre { Id = 6002, Name = "Action Roguelike" },
                //    new Genre { Id = 5829, Name = "Turn-Based Strategy" },
                //    new Genre { Id = 5786, Name = "Tabletop" },
                //    new Genre { Id = 5785, Name = "Interactive Fiction" },
                //    new Genre { Id = 4934, Name = "Education" },
                //    new Genre { Id = 4921, Name = "JRPG" },
                //    new Genre { Id = 4886, Name = "Dating Sim" },
                //    new Genre { Id = 4868, Name = "Party-Based RPG" },
                //    new Genre { Id = 4538, Name = "Walking Simulator" },
                //    new Genre { Id = 4368, Name = "Card Game" },
                //    new Genre { Id = 3886, Name = "Life Sim" },
                //    new Genre { Id = 3250, Name = "Strategy RPG" },
                //    new Genre { Id = 3138, Name = "RTS" },
                //    new Genre { Id = 3137, Name = "Board Game" },
                //    new Genre { Id = 2914, Name = "Tower Defense" },
                //    new Genre { Id = 2626, Name = "City Builder" },
                //    new Genre { Id = 1473, Name = "Farming Sim" },
                //    new Genre { Id = 1393, Name = "Grand Strategy" },
                //    new Genre { Id = 1377, Name = "Space Sim" },
                //    new Genre { Id = 1337, Name = "Colony Sim" },
                //    new Genre { Id = 1290, Name = "eSports" },
                //    new Genre { Id = 1254, Name = "MMORPG" },
                //    new Genre { Id = 1245, Name = "Word Game" },
                //    new Genre { Id = 1237, Name = "Battle Royale" },
                //    new Genre { Id = 1186, Name = "Auto Battler" },
                //    new Genre { Id = 893, Name = "God Game" },
                //    new Genre { Id = 659, Name = "MOBA" },
                //    new Genre { Id = 735, Name = "4X" },
                //    new Genre { Id = 483, Name = "Trivia" }
                //);
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

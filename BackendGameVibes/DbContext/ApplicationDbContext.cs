using Microsoft.EntityFrameworkCore;
using BackendGameVibes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace BackendGameVibes.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserGameVibes>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<UserGameVibes> Users
        {
            get; set;
        }
        public DbSet<ForumRole> ForumRoles
        {
            get; set;
        }
        public DbSet<Role> Roles
        {
            get; set;
        }
        public DbSet<Game> Games
        {
            get; set;
        }
        public DbSet<Platform> Platforms
        {
            get; set;
        }
        public DbSet<Genre> Genres
        {
            get; set;
        }
        public DbSet<GameImage> GameImages
        {
            get; set;
        }
        public DbSet<SystemRequirement> SystemRequirements
        {
            get; set;
        }
        public DbSet<Review> Reviews
        {
            get; set;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.LogTo(message => Debug.WriteLine(message));
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<UserCodeShare>()
            //    .HasMany(u => u.CodeSnippets)
            //    .WithOne(c => c.User)
            //    .HasForeignKey(c => c.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<CodeSnippet>(builder =>
            //{
            //    builder
            //    .HasKey(p => p.Id);

            //    builder
            //    .HasOne(p => p.SelectedLang)
            //    .WithMany()
            //    .HasForeignKey(c => c.SelectedLangId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //    builder
            //    .Property(c => c.SelectedLangId)
            //    .HasDefaultValue(1);

            //    builder.Property(p => p.ExpiryDate)
            //    .HasDefaultValue(DateTime.Now.AddDays(3));
            //});

            //modelBuilder.Entity<ProgLanguage>(builder =>
            //{
            //    builder
            //    .HasKey(p => p.Id);

            //    builder.Property(p => p.Name)
            //    .IsRequired();

            //    builder.HasData(
            //        new ProgLanguage() { Id = 1, Name = "javascript" },
            //        new ProgLanguage() { Id = 2, Name = "xml" },
            //        new ProgLanguage() { Id = 3, Name = "css" },
            //        new ProgLanguage() { Id = 4, Name = "go" },
            //        new ProgLanguage() { Id = 5, Name = "php" },
            //        new ProgLanguage() { Id = 6, Name = "python" },
            //        new ProgLanguage() { Id = 7, Name = "sql" },
            //        new ProgLanguage() { Id = 8, Name = "swift" });
            //});
        }
    }
}

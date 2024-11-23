using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Diagnostics;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.User;
using BackendGameVibes.Models.Reported;
using System.Reflection.Emit;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Models;

namespace BackendGameVibes.Data {
    public class ApplicationDbContext : IdentityDbContext<UserGameVibes> {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }

        public virtual DbSet<Game> Games {
            get; set;
        }
        public virtual DbSet<Platform> Platforms {
            get; set;
        }
        public virtual DbSet<Genre> Genres {
            get; set;
        }
        public virtual DbSet<GameImage> GameImages {
            get; set;
        }
        public virtual DbSet<Review> Reviews {
            get; set;
        }
        public virtual DbSet<ForumThread> ForumThreads {
            get; set;
        }
        public virtual DbSet<ForumPost> ForumPosts {
            get; set;
        }
        public virtual DbSet<ForumSection> ForumSections {
            get; set;
        }
        public virtual DbSet<ForumRole> ForumRoles {
            get; set;
        }
        public virtual DbSet<FriendRequest> FriendRequests {
            get; set;
        }
        public virtual DbSet<Friend> Friends {
            get; set;
        }
        public virtual DbSet<ReportedReview> ReportedReviews {
            get; set;
        }
        public virtual DbSet<ReportedPost> ReportedForumPosts {
            get; set;
        }
        public virtual DbSet<ProfilePicture> ProfilePictures {
            get; set;
        }
        public virtual DbSet<ActionCode> ActiveActionCodes {
            get; set;
        }
        public virtual DbSet<ForumPostInteraction> ForumPostInteractions {
            get; set;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.LogTo(message => Debug.WriteLine(message));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder mB) {
            // UserGameVibes ent
            mB.Entity<UserGameVibes>(ent => {
                ent.HasOne(u => u.ForumRole)
                    .WithMany(fr => fr.Users)
                    .HasForeignKey(u => u.ForumRoleId)
                    .OnDelete(DeleteBehavior.SetNull);

                ent.Property(u => u.Description)
                    .HasMaxLength(500);

                ent.Property(u => u.ExperiencePoints)
                    .HasDefaultValue(10);

                ent.Property(u => u.ForumRoleId)
                    .HasDefaultValue(1);

                ent.Property(u => u.LastActivityDate)
                    .HasColumnType("datetime(6)")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                ent.HasMany(u => u.UserReviews)
                    .WithOne(r => r.UserGameVibes)
                    .HasForeignKey(u => u.UserGameVibesId)
                    .OnDelete(DeleteBehavior.SetNull);

                ent.HasMany(u => u.UserFollowedGames)
                    .WithMany(g => g.PlayersFollowing)
                    .UsingEntity(j => j.ToTable("UsersGamesFollow"));
            });

            // ForumRole ent
            mB.Entity<ForumRole>(ent => {
                ent.HasKey(fr => fr.Id);

                ent.Property(fr => fr.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                ent.Property(fr => fr.Threshold)
                    .IsRequired();

                ent.HasData(
                    new ForumRole() { Id = 1, Name = "beginner", Threshold = 0 },
                    new ForumRole() { Id = 2, Name = "experienced", Threshold = 100 },
                    new ForumRole() { Id = 3, Name = "powerful", Threshold = 1000 },
                    new ForumRole() { Id = 4, Name = "superhero", Threshold = 10000 });
            });

            // Game ent
            mB.Entity<Game>(ent => {
                ent.HasKey(g => g.Id);

                ent.Property(g => g.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                ent.Property(g => g.Description)
                    .HasMaxLength(10000);

                ent.Property(g => g.LastCalculatedRatingFromReviews)
                    .HasPrecision(3, 1)
                    .HasDefaultValue(0.0);

                ent.HasMany(g => g.Platforms)
                    .WithMany(p => p.Games)
                    .UsingEntity(j => j.ToTable("GamesPlatforms")); // many to many so we need 3 table to store the relationship

                ent.HasMany(g => g.Genres)
                    .WithMany(gen => gen.Games)
                    .UsingEntity(j => j.ToTable("GamesGenres")); // many to many so we need 3 table to store the relationship

                ent.HasMany(g => g.GameImages)
                    .WithOne(img => img.Game)
                    .HasForeignKey(img => img.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Platform ent
            mB.Entity<Platform>(ent => {
                ent.HasKey(p => p.Id);

                ent.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                ent.HasData(
                    new Platform() { Id = 1, Name = "Windows" },
                    new Platform() { Id = 2, Name = "Linux" },
                    new Platform() { Id = 3, Name = "MacOS" });
            });

            // Genre ent
            mB.Entity<Genre>(ent => {
                ent.HasKey(g => g.Id);

                ent.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                // steam Ids with their corresponding name genres, jednak tego jest duzo wiecej więc z automatu będą się dodawać niewystępujące jeszcze w bazie
            });

            // GameImage ent
            mB.Entity<GameImage>(ent => {
                ent.HasKey(gi => gi.Id);

                ent.Property(gi => gi.ImagePath)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            // Review ent
            mB.Entity<Review>(ent => {
                ent.HasKey(r => r.Id);

                ent.Property(r => r.GeneralScore)
                    .IsRequired()
                    .HasPrecision(3, 2);

                ent.Property(r => r.GraphicsScore)
                    .IsRequired()
                    .HasPrecision(3, 2);

                ent.Property(r => r.AudioScore)
                    .IsRequired()
                    .HasPrecision(3, 2);

                ent.Property(r => r.GameplayScore)
                    .IsRequired()
                    .HasPrecision(3, 2);

                ent.Property(r => r.Comment)
                    .HasMaxLength(1000);

                ent.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                    .IsRequired();

                ent.Property(r => r.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                    .IsRequired();

                ent.Property(r => r.AverageRating)
                    .HasPrecision(3, 1);

                ent.HasOne(r => r.Game)
                    .WithMany(g => g.Reviews)
                    .HasForeignKey(r => r.GameId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            mB.Entity<ForumSection>(ent => {
                ent.HasData(
                    new ForumSection() { Id = 1, Name = "general", HexColor = "#d98600" },
                    new ForumSection() { Id = 2, Name = "technologies", HexColor = "#00b1d9" },
                    new ForumSection() { Id = 3, Name = "offtopic", HexColor = "#1cad0c" },
                    new ForumSection() { Id = 4, Name = "advices", HexColor = "#91248e" });
            });

            mB.Entity<ForumThread>(ent => {
                ent.HasKey(t => t.Id);

                ent.Property(t => t.CreatedDateTime)
                    .HasColumnType("datetime(6)")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                ent.Property(t => t.LastUpdatedDateTime)
                    .HasColumnType("datetime(6)")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                    .IsRequired()
                    .ValueGeneratedOnAddOrUpdate();

                ent.HasOne(t => t.UserOwner)
                    .WithMany(u => u.UserForumThreads)
                    .HasForeignKey(t => t.UserOwnerId)
                    .OnDelete(DeleteBehavior.SetNull);

                ent.HasOne(t => t.Section)
                    .WithMany(s => s.Threads)
                    .HasForeignKey(t => t.SectionId)
                    .OnDelete(DeleteBehavior.SetNull);

                ent.HasMany(t => t.Posts)
                    .WithOne(p => p.Thread)
                    .HasForeignKey(p => p.ThreadId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            mB.Entity<ForumPost>(ent => {
                ent.HasKey(p => p.Id);

                ent.Property(p => p.CreatedDateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                ent.Property(p => p.LastUpdatedDateTime)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
                    .IsRequired()
                    .ValueGeneratedOnAddOrUpdate();

                ent.Property(p => p.LikesCount)
                    .HasDefaultValue(0);

                ent.Property(p => p.DisLikesCount)
                    .HasDefaultValue(0);

                ent.HasOne(p => p.UserOwner)
                    .WithMany(u => u.UserForumPosts)
                    .HasForeignKey(p => p.UserOwnerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });


            mB.Entity<FriendRequest>(ent => {
                ent.Property(fR => fR.SentAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                ent.HasOne(fr => fr.SenderUser)
                   .WithMany(u => u.UserFriendRequestsSent)
                   .HasForeignKey(fr => fr.SenderUserId)
                   .OnDelete(DeleteBehavior.Cascade);

                ent.HasOne(fr => fr.ReceiverUser)
                    .WithMany(u => u.UserFriendRequestsReceived)
                    .HasForeignKey(fr => fr.ReceiverUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            mB.Entity<Friend>(ent => {
                ent.Property(fR => fR.FriendsSince)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                ent.HasOne(f => f.User)
                    .WithMany(u => u.UserFriends)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                ent.HasOne(f => f.FriendUser)
                    .WithMany()
                    .HasForeignKey(f => f.FriendId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            mB.Entity<ProfilePicture>(ent => {
                ent.HasOne(p => p.User)
                .WithOne(u => u.ProfilePicture)
                .HasForeignKey<ProfilePicture>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            mB.Entity<ReportedReview>(ent => {
                ent.HasOne(rr => rr.Review)
                    .WithMany(r => r.ReportedReviews)
                    .HasForeignKey(rr => rr.ReviewId)
                    .OnDelete(DeleteBehavior.SetNull);

                ent.HasOne(rr => rr.ReporterUser)
                    .WithMany(u => u.UserReportedReviews)
                    .HasForeignKey(rr => rr.ReporterUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                ent.Property(rr => rr.Reason)
                    .HasMaxLength(255)
                    .IsRequired();
            });

            mB.Entity<ReportedPost>(ent => {
                ent.HasOne(rr => rr.ForumPost)
                    .WithMany(r => r.ReportsToThisPosts)
                    .HasForeignKey(rr => rr.ForumPostId)
                    .OnDelete(DeleteBehavior.SetNull);

                ent.HasOne(rr => rr.ReporterUser)
                    .WithMany(u => u.UserReportedPosts)
                    .HasForeignKey(rr => rr.ReporterUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                ent.Property(rr => rr.Reason)
                    .HasMaxLength(255)
                    .IsRequired();
            });

            mB.Entity<ForumPostInteraction>(ent => {
                ent.HasOne(fpi => fpi.UserGameVibes)
                    .WithMany(u => u.UserPostInteractions)
                    .HasForeignKey(fpi => fpi.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                ent.HasOne(fpi => fpi.ForumPost)
                    .WithMany(p => p.PostInteractions)
                    .HasForeignKey(fpi => fpi.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                ent.Property(fpi => fpi.IsLike)
                    .HasDefaultValue(null);



            });

            base.OnModelCreating(mB);
        }

    }
}

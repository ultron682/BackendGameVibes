﻿// <auto-generated />
using System;
using BackendGameVibes.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BackendGameVibes.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("BackendGameVibes.Models.Forum.ForumPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("DisLikes")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<DateTime>("LastUpdatedDateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("Likes")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<int?>("ThreadId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserOwnerId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ThreadId");

                    b.HasIndex("UserOwnerId");

                    b.ToTable("ForumPosts");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Forum.ForumRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int?>("Threshold")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ForumRoles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "beginner",
                            Threshold = 0
                        },
                        new
                        {
                            Id = 2,
                            Name = "experienced",
                            Threshold = 100
                        },
                        new
                        {
                            Id = 3,
                            Name = "powerful",
                            Threshold = 1000
                        },
                        new
                        {
                            Id = 4,
                            Name = "superhero",
                            Threshold = 10000
                        });
                });

            modelBuilder.Entity("BackendGameVibes.Models.Forum.ForumSection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ForumSections");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "general"
                        },
                        new
                        {
                            Id = 2,
                            Name = "technologies"
                        },
                        new
                        {
                            Id = 3,
                            Name = "offtopic"
                        },
                        new
                        {
                            Id = 4,
                            Name = "advices"
                        });
                });

            modelBuilder.Entity("BackendGameVibes.Models.Forum.ForumThread", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime>("LastUpdatedDateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int?>("SectionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserOwnerId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.HasIndex("UserOwnerId");

                    b.ToTable("ForumThreads");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Friends.Friend", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FriendId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FriendsSince")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FriendId");

                    b.HasIndex("UserId");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Friends.FriendRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsAccepted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ReceiverUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SenderUserId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("SentAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverUserId");

                    b.HasIndex("SenderUserId");

                    b.ToTable("FriendRequests");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Games.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CoverImage")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<DateOnly?>("ReleaseDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("SteamId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Games.GameImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GameImages");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Games.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Games.Platform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Platforms");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Windows"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Linux"
                        },
                        new
                        {
                            Id = 3,
                            Name = "MacOS"
                        });
                });

            modelBuilder.Entity("BackendGameVibes.Models.Games.SystemRequirement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CpuRequirement")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("DiskRequirement")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GpuRequirement")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("OperatingSystemRequirement")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("RamRequirement")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("SystemRequirements");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Reported.ReportedPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ForumPostId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("ReporterUserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ForumPostId");

                    b.HasIndex("ReporterUserId");

                    b.ToTable("ReportedPosts");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Reported.ReportedReview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("ReporterUserId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ReviewId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ReporterUserId");

                    b.HasIndex("ReviewId");

                    b.ToTable("ReportedReviews");
                });

            modelBuilder.Entity("BackendGameVibes.Models.User.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("AudioScore")
                        .HasPrecision(3, 2)
                        .HasColumnType("REAL");

                    b.Property<string>("Comment")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int?>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("GameplayScore")
                        .HasPrecision(3, 2)
                        .HasColumnType("REAL");

                    b.Property<double>("GeneralScore")
                        .HasPrecision(3, 2)
                        .HasColumnType("REAL");

                    b.Property<double>("GraphicsScore")
                        .HasPrecision(3, 2)
                        .HasColumnType("REAL");

                    b.Property<string>("UserGameVibesId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("UserGameVibesId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("BackendGameVibes.Models.User.UserGameVibes", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ExperiencePoints")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(10);

                    b.Property<int?>("ForumRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(1);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("ProfilePicture")
                        .HasColumnType("BLOB");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ForumRoleId");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("GameGenre", b =>
                {
                    b.Property<int>("GamesId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GenresId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GamesId", "GenresId");

                    b.HasIndex("GenresId");

                    b.ToTable("GamesGenres", (string)null);
                });

            modelBuilder.Entity("GamePlatform", b =>
                {
                    b.Property<int>("GamesId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlatformsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GamesId", "PlatformsId");

                    b.HasIndex("PlatformsId");

                    b.ToTable("GamesPlatforms", (string)null);
                });

            modelBuilder.Entity("GameUserGameVibes", b =>
                {
                    b.Property<string>("PlayersFollowingId")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserFollowedGamesId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlayersFollowingId", "UserFollowedGamesId");

                    b.HasIndex("UserFollowedGamesId");

                    b.ToTable("UsersGamesFollow", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("BackendGameVibes.Models.Forum.ForumPost", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Forum.ForumThread", "Thread")
                        .WithMany("Posts")
                        .HasForeignKey("ThreadId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", "UserOwner")
                        .WithMany("UserForumPosts")
                        .HasForeignKey("UserOwnerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Thread");

                    b.Navigation("UserOwner");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Forum.ForumThread", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Forum.ForumSection", "Section")
                        .WithMany("Threads")
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", "UserOwner")
                        .WithMany("UserForumThreads")
                        .HasForeignKey("UserOwnerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Section");

                    b.Navigation("UserOwner");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Friends.Friend", b =>
                {
                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", "FriendUser")
                        .WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", "User")
                        .WithMany("UserFriends")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("FriendUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Friends.FriendRequest", b =>
                {
                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", "ReceiverUser")
                        .WithMany("UserFriendRequestsReceived")
                        .HasForeignKey("ReceiverUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", "SenderUser")
                        .WithMany("UserFriendRequestsSent")
                        .HasForeignKey("SenderUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("ReceiverUser");

                    b.Navigation("SenderUser");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Games.GameImage", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Games.Game", "Game")
                        .WithMany("GameImages")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Game");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Games.SystemRequirement", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Games.Game", "Game")
                        .WithMany("SystemRequirements")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Reported.ReportedPost", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Forum.ForumPost", "ForumPost")
                        .WithMany("ReportedPosts")
                        .HasForeignKey("ForumPostId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", "ReporterUser")
                        .WithMany("UserReportedPosts")
                        .HasForeignKey("ReporterUserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("ForumPost");

                    b.Navigation("ReporterUser");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Reported.ReportedReview", b =>
                {
                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", "ReporterUser")
                        .WithMany("UserReportedReviews")
                        .HasForeignKey("ReporterUserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("BackendGameVibes.Models.User.Review", "Review")
                        .WithMany("ReportedReviews")
                        .HasForeignKey("ReviewId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("ReporterUser");

                    b.Navigation("Review");
                });

            modelBuilder.Entity("BackendGameVibes.Models.User.Review", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Games.Game", "Game")
                        .WithMany("Reviews")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", "UserGameVibes")
                        .WithMany("UserReviews")
                        .HasForeignKey("UserGameVibesId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Game");

                    b.Navigation("UserGameVibes");
                });

            modelBuilder.Entity("BackendGameVibes.Models.User.UserGameVibes", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Forum.ForumRole", "ForumRole")
                        .WithMany("Users")
                        .HasForeignKey("ForumRoleId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("ForumRole");
                });

            modelBuilder.Entity("GameGenre", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Games.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendGameVibes.Models.Games.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GamePlatform", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Games.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendGameVibes.Models.Games.Platform", null)
                        .WithMany()
                        .HasForeignKey("PlatformsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GameUserGameVibes", b =>
                {
                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", null)
                        .WithMany()
                        .HasForeignKey("PlayersFollowingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendGameVibes.Models.Games.Game", null)
                        .WithMany()
                        .HasForeignKey("UserFollowedGamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("BackendGameVibes.Models.User.UserGameVibes", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BackendGameVibes.Models.Forum.ForumPost", b =>
                {
                    b.Navigation("ReportedPosts");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Forum.ForumRole", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Forum.ForumSection", b =>
                {
                    b.Navigation("Threads");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Forum.ForumThread", b =>
                {
                    b.Navigation("Posts");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Games.Game", b =>
                {
                    b.Navigation("GameImages");

                    b.Navigation("Reviews");

                    b.Navigation("SystemRequirements");
                });

            modelBuilder.Entity("BackendGameVibes.Models.User.Review", b =>
                {
                    b.Navigation("ReportedReviews");
                });

            modelBuilder.Entity("BackendGameVibes.Models.User.UserGameVibes", b =>
                {
                    b.Navigation("UserForumPosts");

                    b.Navigation("UserForumThreads");

                    b.Navigation("UserFriendRequestsReceived");

                    b.Navigation("UserFriendRequestsSent");

                    b.Navigation("UserFriends");

                    b.Navigation("UserReportedPosts");

                    b.Navigation("UserReportedReviews");

                    b.Navigation("UserReviews");
                });
#pragma warning restore 612, 618
        }
    }
}

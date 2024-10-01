﻿// <auto-generated />
using System;
using BackendGameVibes.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BackendGameVibes.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241001142323_newdefaults")]
    partial class newdefaults
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BackendGameVibes.Models.ForumRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("Threshold")
                        .IsRequired()
                        .HasColumnType("int");

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

            modelBuilder.Entity("BackendGameVibes.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("PlatformId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("PlatformId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("BackendGameVibes.Models.GameImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("GameId")
                        .HasColumnType("int");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GameImages");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Genres");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Action"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Adventure"
                        },
                        new
                        {
                            Id = 3,
                            Name = "RPG"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Shooter"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Strategy"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Sports"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Puzzle"
                        },
                        new
                        {
                            Id = 8,
                            Name = "Simulation"
                        },
                        new
                        {
                            Id = 9,
                            Name = "Horror"
                        },
                        new
                        {
                            Id = 10,
                            Name = "Platformer"
                        },
                        new
                        {
                            Id = 11,
                            Name = "Racing"
                        },
                        new
                        {
                            Id = 12,
                            Name = "Fighting"
                        },
                        new
                        {
                            Id = 13,
                            Name = "MMO"
                        },
                        new
                        {
                            Id = 14,
                            Name = "Survival"
                        },
                        new
                        {
                            Id = 15,
                            Name = "Music"
                        },
                        new
                        {
                            Id = 16,
                            Name = "Stealth"
                        },
                        new
                        {
                            Id = 17,
                            Name = "Party"
                        },
                        new
                        {
                            Id = 18,
                            Name = "Arcade"
                        },
                        new
                        {
                            Id = 19,
                            Name = "Sandbox"
                        },
                        new
                        {
                            Id = 20,
                            Name = "Card Game"
                        });
                });

            modelBuilder.Entity("BackendGameVibes.Models.Platform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

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

            modelBuilder.Entity("BackendGameVibes.Models.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("AudioScore")
                        .HasPrecision(3, 2)
                        .HasColumnType("float(3)");

                    b.Property<string>("Comment")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<double>("GameplayScore")
                        .HasPrecision(3, 2)
                        .HasColumnType("float(3)");

                    b.Property<double>("GeneralScore")
                        .HasPrecision(3, 2)
                        .HasColumnType("float(3)");

                    b.Property<double>("GraphicsScore")
                        .HasPrecision(3, 2)
                        .HasColumnType("float(3)");

                    b.Property<string>("UserGameVibesId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("UserGameVibesId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "guest"
                        },
                        new
                        {
                            Id = 2,
                            Name = "user"
                        },
                        new
                        {
                            Id = 3,
                            Name = "mod"
                        },
                        new
                        {
                            Id = 4,
                            Name = "admin"
                        });
                });

            modelBuilder.Entity("BackendGameVibes.Models.SystemRequirement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CpuRequirement")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("DiskRequirement")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<string>("GpuRequirement")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("OperatingSystemRequirement")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RamRequirement")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("SystemRequirements");
                });

            modelBuilder.Entity("BackendGameVibes.Models.UserGameVibes", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<int?>("ExperiencePoints")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<int?>("ForumRoleId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<byte[]>("ProfilePicture")
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("RoleId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("ForumRoleId");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("GameGenre", b =>
                {
                    b.Property<int>("GamesId")
                        .HasColumnType("int");

                    b.Property<int>("GenresId")
                        .HasColumnType("int");

                    b.HasKey("GamesId", "GenresId");

                    b.HasIndex("GenresId");

                    b.ToTable("GameGenres", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("BackendGameVibes.Models.Game", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Platform", "Platform")
                        .WithMany("Games")
                        .HasForeignKey("PlatformId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Platform");
                });

            modelBuilder.Entity("BackendGameVibes.Models.GameImage", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Game", "Game")
                        .WithMany("GameImages")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Game");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Review", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendGameVibes.Models.UserGameVibes", "UserGameVibes")
                        .WithMany()
                        .HasForeignKey("UserGameVibesId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Game");

                    b.Navigation("UserGameVibes");
                });

            modelBuilder.Entity("BackendGameVibes.Models.SystemRequirement", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Game", "Game")
                        .WithMany("SystemRequirements")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("BackendGameVibes.Models.UserGameVibes", b =>
                {
                    b.HasOne("BackendGameVibes.Models.ForumRole", "ForumRole")
                        .WithMany("Users")
                        .HasForeignKey("ForumRoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BackendGameVibes.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ForumRole");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("GameGenre", b =>
                {
                    b.HasOne("BackendGameVibes.Models.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BackendGameVibes.Models.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
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
                    b.HasOne("BackendGameVibes.Models.UserGameVibes", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("BackendGameVibes.Models.UserGameVibes", null)
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

                    b.HasOne("BackendGameVibes.Models.UserGameVibes", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("BackendGameVibes.Models.UserGameVibes", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BackendGameVibes.Models.ForumRole", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Game", b =>
                {
                    b.Navigation("GameImages");

                    b.Navigation("SystemRequirements");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Platform", b =>
                {
                    b.Navigation("Games");
                });

            modelBuilder.Entity("BackendGameVibes.Models.Role", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}

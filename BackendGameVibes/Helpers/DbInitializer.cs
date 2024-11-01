using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.Requests.Forum;
using BackendGameVibes.Models.Requests.Reported;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Models.User;
using BackendGameVibes.Services.Forum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Runtime.CompilerServices;

namespace BackendGameVibes.Helpers {
    public class DbInitializer {
        private static readonly string[] LoremIpsumWords = {
        "lorem", "ipsum", "dolor", "sit", "amet", "consectetur",
        "adipiscing", "elit", "sed", "do", "eiusmod", "tempor", "incididunt",
        "ut", "labore", "et", "dolore", "magna", "aliqua"
       };

        public static string GenerateRandomSentence(int wordCount) {
            Random random = new Random();
            var words = new List<string>();

            for (int i = 0; i < wordCount; i++) {
                var word = LoremIpsumWords[random.Next(LoremIpsumWords.Length)];
                words.Add(word);
            }

            return string.Join(" ", words) + ".";
        }

        public static async Task InitializeAsync(AsyncServiceScope scope, ApplicationDbContext applicationDbContext) {
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserGameVibes>>();
            using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            bool roleExist = await roleManager.RoleExistsAsync("admin");
            if (roleExist)
                return;

            using var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            using var reviewService = scope.ServiceProvider.GetRequiredService<IReviewService>();
            using var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
            using var threadService = scope.ServiceProvider.GetService<IForumThreadService>();
            using var postService = scope.ServiceProvider.GetService<IForumPostService>();
            Random random = new();

            Console.WriteLine("Start Init DB");

            Console.WriteLine("Adding users with roles");
            if (!roleExist) {
                var role = new IdentityRole();
                role.Name = "admin";
                await roleManager.CreateAsync(role);

                //Admin           
                var newUser = new UserGameVibes {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    EmailConfirmed = true
                };
                string userPWD = "Admin123.";

                IdentityResult chkUser = await userManager.CreateAsync(newUser, userPWD);

                newUser = await userManager.FindByEmailAsync(newUser.Email);
                var result1 = await userManager.AddToRoleAsync(newUser!, "admin");

            }

            // Creating mod role     
            roleExist = await roleManager.RoleExistsAsync("mod");
            if (!roleExist) {
                var role = new IdentityRole();
                role.Name = "mod";
                await roleManager.CreateAsync(role);

                //Mod         
                var newUser = new UserGameVibes {
                    UserName = "mod",
                    Email = "mod@mod.com",
                    EmailConfirmed = true
                };
                string userPWD = "Mod123.";

                IdentityResult chkUser = await userManager.CreateAsync(newUser, userPWD);

                if (chkUser.Succeeded) {
                    var result1 = await userManager.AddToRoleAsync(newUser, "mod");
                }
            }

            // Creating user role     
            roleExist = await roleManager.RoleExistsAsync("user");
            if (!roleExist) {
                var role = new IdentityRole();
                role.Name = "user";
                await roleManager.CreateAsync(role);

                HashSet<UserGameVibes> newNotCreatedUsersGameVibes = new() {
                    new UserGameVibes {
                        UserName = "test",
                        Email = "test@test.com",
                        EmailConfirmed = true
                    },
                    new UserGameVibes {
                        UserName = "test2",
                        Email = "test2@test.com",
                        EmailConfirmed = true
                    }, new UserGameVibes {
                        UserName = "test3",
                        Email = "test3@test.com",
                        EmailConfirmed = true
                    }, new UserGameVibes {
                        UserName = "test4",
                        Email = "test4@test.com",
                        EmailConfirmed = true
                    }, new UserGameVibes {
                        UserName = "test5",
                        Email = "test5@test.com",
                        EmailConfirmed = true
                    },new UserGameVibes {
                        UserName = "test6",
                        Email = "test6@test.com",
                        EmailConfirmed = true
                    }
                };

                string userPWD = "Test123.";

                foreach (var user in newNotCreatedUsersGameVibes) {
                    IdentityResult chkUser = await userManager.CreateAsync(user, userPWD);

                    if (chkUser.Succeeded) {
                        await userManager.AddToRoleAsync(user, "user");
                    }
                }
            }

            Console.WriteLine("Adding friends");

            var testUsers = new UserGameVibes?[] {
                await userManager.FindByEmailAsync("test@test.com"),
                await userManager.FindByEmailAsync("test2@test.com"),
                await userManager.FindByEmailAsync("test3@test.com"),
                await userManager.FindByEmailAsync("test4@test.com"),
                await userManager.FindByEmailAsync("test5@test.com"),
                await userManager.FindByEmailAsync("test6@test.com"),
            };


            var friendRequest1 = new FriendRequest {
                SenderUserId = testUsers[0]!.Id,
                ReceiverUserId = testUsers[1]!.Id,
                IsAccepted = true
            };
            applicationDbContext.FriendRequests.Add(friendRequest1);

            var friendRequest2 = new FriendRequest {
                SenderUserId = testUsers[0]!.Id,
                ReceiverUserId = testUsers[2]!.Id,
                IsAccepted = true
            };
            applicationDbContext.FriendRequests.Add(friendRequest2);


            var friendRequest3 = new FriendRequest {
                SenderUserId = testUsers[0]!.Id,
                ReceiverUserId = testUsers[3]!.Id,
                IsAccepted = true
            };
            applicationDbContext.FriendRequests.Add(friendRequest3);



            var friend1 = new Friend { UserId = testUsers[0]!.Id, FriendId = testUsers[1]!.Id };
            var friend2 = new Friend { UserId = testUsers[1]!.Id, FriendId = testUsers[0]!.Id };
            applicationDbContext.Friends.AddRange(friend1, friend2);

            var friend3 = new Friend { UserId = testUsers[0]!.Id, FriendId = testUsers[2]!.Id };
            var friend4 = new Friend { UserId = testUsers[2]!.Id, FriendId = testUsers[0]!.Id };
            applicationDbContext.Friends.AddRange(friend3, friend4);

            var friend5 = new Friend { UserId = testUsers[0]!.Id, FriendId = testUsers[3]!.Id };
            var friend6 = new Friend { UserId = testUsers[3]!.Id, FriendId = testUsers[0]!.Id };
            applicationDbContext.Friends.AddRange(friend5, friend6);

            await applicationDbContext.SaveChangesAsync();

            Console.WriteLine("Adding games");
            HashSet<int> steamGameIds = [
                20900, // The Witcher 1
                20920, // The Witcher 2, wither 3 292030 is added below IN SEPARETE CALL
                1593500, // God of war 1
                2322010, // God of war 2
                1222670, // The Sims™ 4
                47890, // The Sims™ 3
                256321, // LEGO MARVEL Super Heroes DLC: Asgard Pack
                231430, // Company of Heroes 2
                3035570, // Assassin's Creed Mirage
                934700, //Dead Island 2
                // Upcoming games:
                3191990, // Tiny House Simulator 2024-11-05
                2651280, // Marvel's Spider-Man 2
                2246340, // Monster Hunter Wilds
                1850050, // Alien: Rogue Incursion
                2671160, // Galactic Civilizations IV - Megastructures
                2677660, // Indiana Jones i Wielki Krąg
                2767030, // Marvel Rivals
                220, // Half-Life 2
                440, // Team Fortress 2
                730, // Counter-Strike 2
                570, // Dota 2
                8930, // Sid Meier's Civilization® V
                219640, // Chivalry: Medieval Warfare
                245620, // Tropico 5
                43110, // Metro 2033
                105600, // Terraria
            ];

            (Game? game, bool isSuccess) createdGame1 = await gameService.CreateGame(292030); // The Witcher 3

            var createdGames = new List<(Game? game, bool isSuccess)>();

            foreach (var gameId in steamGameIds) {
                createdGames.Add(await gameService.CreateGame(gameId));
            }

            if (createdGame1.game != null) {
                await reviewService.AddReviewAsync(new Review {
                    GameId = createdGame1!.game.Id,
                    Comment = "Great game. Review #1 created automatic in Program.cs",
                    GeneralScore = 9.5,
                    GraphicsScore = 7.5,
                    AudioScore = 6.5,
                    GameplayScore = 8.9,
                    UserGameVibesId = testUsers[0]!.Id
                });

                await reviewService.AddReviewAsync(new Review {
                    GameId = createdGame1!.game.Id,
                    Comment = "Great game in my life. Review #2 created automatic in Program.cs",
                    GeneralScore = 9.5,
                    GraphicsScore = 7.5,
                    AudioScore = 6.5,
                    GameplayScore = 8.9,
                    UserGameVibesId = testUsers[1]!.Id
                });

                for (int i = 0; i < 100; i++) {
                    await reviewService.AddReviewAsync(new Review {
                        GameId = createdGames[random.Next(0, createdGames.Count)].game!.Id,
                        Comment = GenerateRandomSentence(random.Next(10, 30)),
                        GeneralScore = random.Next(1, 11), // 1-10
                        GraphicsScore = random.Next(1, 11),
                        AudioScore = random.Next(1, 11),
                        GameplayScore = random.Next(1, 11),
                        UserGameVibesId = testUsers[random.Next(0, testUsers.Length)]!.Id
                    });
                }
            }
            Console.WriteLine("Adding threads with posts");
            // user #1 threads
            for (int i = 0; i < 9; i++) {
                ForumThread newForumThread = await threadService!.AddThreadAsync(new NewForumThreadDTO {
                    Title = $"Forum Thread {i} " + GenerateRandomSentence(2),
                    UserOwnerId = testUsers[0]!.Id,
                    SectionId = 1,
                    FirstForumPostContent = GenerateRandomSentence(random.Next(10, 30))
                });

                int postsCount = random.Next(0, 10);
                for (int j = 0; j < postsCount; j++) {
                    await postService!.AddForumPost(new ForumPostDTO {
                        Content = $"{postsCount} " + GenerateRandomSentence(random.Next(10, 30)),
                        ThreadId = newForumThread.Id,
                        UserOwnerId = testUsers[random.Next(0, testUsers.Length)]!.Id
                    });
                }
            }

            // user #2 threads 
            for (int i = 0; i < 9; i++) {
                ForumThread newForumThread = await threadService!.AddThreadAsync(new NewForumThreadDTO {
                    Title = $"Forum Thread {i} " + GenerateRandomSentence(2),
                    UserOwnerId = testUsers[1]!.Id,
                    SectionId = 1,
                    FirstForumPostContent = GenerateRandomSentence(random.Next(10, 30))
                });

                int postsCount = random.Next(0, 10);
                for (int j = 0; j < postsCount; j++) {
                    await postService!.AddForumPost(new ForumPostDTO {
                        Content = $"{postsCount} " + GenerateRandomSentence(random.Next(10, 30)),
                        ThreadId = newForumThread.Id,
                        UserOwnerId = testUsers[random.Next(0, testUsers.Length)]!.Id
                    });
                }
            }

            Console.WriteLine("Adding review,post reports");
            var reviews = await applicationDbContext.Reviews.ToListAsync();
            var posts = await applicationDbContext.ForumPosts.ToListAsync();

            for (int i = 0; i < 10; i++) {
                await postService!.ReportPostAsync(testUsers[random.Next(0, testUsers.Length)]!.Id!,
                    new ReportPostDTO {
                        ForumPostId = posts[random.Next(0, posts.Count)].Id,
                        Reason = "Spam " + GenerateRandomSentence(random.Next(5, 9))
                    });

                await reviewService.ReportReviewAsync(testUsers[random.Next(0, testUsers.Length)]!.Id!,
                    new ReportReviewDTO {
                        ReviewId = reviews[random.Next(0, reviews.Count)].Id,
                        Reason = "Spam " + GenerateRandomSentence(random.Next(5, 9))
                    });
            }

            Console.WriteLine("Finished Init DB");
        }
    }
}

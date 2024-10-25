using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using BackendGameVibes.Models.Forum;
using BackendGameVibes.Models.Requests.Forum;
using BackendGameVibes.Services;
using Microsoft.AspNetCore.Identity;
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

        public static async Task InitializeAsync(AsyncServiceScope scope) {
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserGameVibes>>();

            if (await userManager.FindByEmailAsync("test@test.com") != null)
                return;

            using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            using var reviewService = scope.ServiceProvider.GetRequiredService<IReviewService>();
            using var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
            using var threadService = scope.ServiceProvider.GetService<ThreadService>();
            using var postService = scope.ServiceProvider.GetService<PostService>();
            Random random = new();


            Console.WriteLine("Start Init DB");

            bool x = await roleManager.RoleExistsAsync("admin");
            if (!x) {
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
            x = await roleManager.RoleExistsAsync("mod");
            if (!x) {
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
            x = await roleManager.RoleExistsAsync("user");
            if (!x) {
                var role = new IdentityRole();
                role.Name = "user";
                await roleManager.CreateAsync(role);

                //Normal user      
                var newUser = new UserGameVibes {
                    UserName = "user",
                    Email = "test@test.com",
                    EmailConfirmed = true
                };
                string userPWD = "Test123.";

                IdentityResult chkUser = await userManager.CreateAsync(newUser, userPWD);

                if (chkUser.Succeeded) {
                    await userManager.AddToRoleAsync(newUser, "user");
                }
            }

            // Creating guest role - something like [AllowAnonymous], todo: remove???
            x = await roleManager.RoleExistsAsync("guest");
            if (!x) {
                var role = new IdentityRole();
                role.Name = "guest";
                await roleManager.CreateAsync(role);
            }

            UserGameVibes? userTest = await userManager.FindByEmailAsync("test@test.com");

            (Game? game, bool isSuccess) createdGame1 = await gameService.CreateGame(292030); // The Witcher 3

            var createdGames = new List<(Game? game, bool isSuccess)>() {
                await gameService.CreateGame(20900), // The Witcher 1
                await gameService.CreateGame(20920), // The Witcher 2
                await gameService.CreateGame(1593500), // God of war 1
                await gameService.CreateGame(2322010), // God of war 2
                await gameService.CreateGame(1222670), // The Sims™ 4
                await gameService.CreateGame(47890), // The Sims™ 3
                await gameService.CreateGame(256321), // LEGO MARVEL Super Heroes DLC: Asgard Pack
                await gameService.CreateGame(231430), // Company of Heroes 2
                await gameService.CreateGame(3035570), // Assassin's Creed Mirage
                await gameService.CreateGame(934700), //Dead Island 2
                // Upcoming games:
                await gameService.CreateGame(3191990), // Tiny House Simulator 2024-11-05
                await gameService.CreateGame(2651280), // Marvel's Spider-Man 2
                await gameService.CreateGame(2246340), // Monster Hunter Wilds
                await gameService.CreateGame(1850050), // Alien: Rogue Incursion
                //await gameService.CreateGame(2671160), // Galactic Civilizations IV - Megastructures
                await gameService.CreateGame(2677660), // Indiana Jones i Wielki Krąg
                await gameService.CreateGame(2767030), // Marvel Rivals
            };


            if (createdGame1.game != null) {
                await reviewService.AddReviewAsync(new Review {
                    GameId = createdGame1!.game.Id,
                    Comment = "Great game. Review #1 created automatic in Program.cs",
                    GeneralScore = 9.5,
                    GraphicsScore = 7.5,
                    AudioScore = 6.5,
                    GameplayScore = 8.9,
                    UserGameVibesId = userTest!.Id
                });

                await reviewService.AddReviewAsync(new Review {
                    GameId = createdGame1!.game.Id,
                    Comment = "Great game in my life. Review #2 created automatic in Program.cs",
                    GeneralScore = 9.5,
                    GraphicsScore = 7.5,
                    AudioScore = 6.5,
                    GameplayScore = 8.9,
                    UserGameVibesId = userTest!.Id
                });

                for (int i = 0; i < 100; i++) {
                    await reviewService.AddReviewAsync(new Review {
                        GameId = createdGames[random.Next(0, createdGames.Count)].game!.Id,
                        Comment = GenerateRandomSentence(random.Next(10, 30)),
                        GeneralScore = random.Next(1, 11), // 1-10
                        GraphicsScore = random.Next(1, 11),
                        AudioScore = random.Next(1, 11),
                        GameplayScore = random.Next(1, 11),
                        UserGameVibesId = userTest!.Id
                    });
                }
            }

            for (int i = 0; i < 15; i++) {
                ForumThread newForumThread = await threadService!.AddThreadAsync(new NewForumThreadDTO {
                    Title = $"Forum Thread {i} " + GenerateRandomSentence(2),
                    UserOwnerId = userTest!.Id,
                    SectionId = 1,
                    FirstForumPostContent = GenerateRandomSentence(random.Next(10, 30))
                });

                int postsCount = random.Next(0, 10);
                for (int j = 0; j < postsCount; j++) {
                    await postService!.AddForumPost(new ForumPostDTO {
                        Content = $"{postsCount} " + GenerateRandomSentence(random.Next(10, 30)),
                        ThreadId = newForumThread.Id,
                        UserOwnerId = userTest!.Id
                    });
                }
            }

            Console.WriteLine("Finish Init DB");
        }
    }
}

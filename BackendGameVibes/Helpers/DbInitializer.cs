using BackendGameVibes.Data;
using BackendGameVibes.IServices;
using BackendGameVibes.Models.Friends;
using BackendGameVibes.Models.Games;
using BackendGameVibes.Models.DTOs.Forum;
using BackendGameVibes.Models.DTOs.Reported;
using BackendGameVibes.Models.Reviews;
using BackendGameVibes.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BackendGameVibes.IServices.Forum;

namespace BackendGameVibes.Helpers;

public class DbInitializer {
    public static string GenerateRandomSentence(int wordCount) {
        Random random = new Random();
        var words = new List<string>();

        for (int i = 0; i < wordCount; i++) {
            var word = DbInitializerData.LoremIpsumWords[random.Next(DbInitializerData.LoremIpsumWords.Length)];
            words.Add(word);
        }

        return string.Join(" ", words) + ".";
    }

    public static async Task InitializeAsync(AsyncServiceScope scope, ApplicationDbContext applicationDbContext) {
        try {
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
            using var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();

            Random random = new();
            DateTime startTime = DateTime.Now;
            Console.WriteLine("Start Init DB");

            Console.WriteLine("Adding users with roles");
            var defaultProfileImagePath = Path.Combine("wwwroot/Images", "default-profile.jpg");
            List<UserGameVibes?> testUsers = [];


            if (!roleExist) {
                var role = new IdentityRole();
                role.Name = "admin";
                await roleManager.CreateAsync(role);

                //Admin           
                var newProfilePicture = new ProfilePicture { ImageData = await File.ReadAllBytesAsync(defaultProfileImagePath) };
                var newUser = new UserGameVibes {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    ProfilePicture = new ProfilePicture { ImageData = await File.ReadAllBytesAsync(defaultProfileImagePath) }
                };
                string userPWD = "Test123.";

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
                var newProfilePicture = new ProfilePicture { ImageData = await File.ReadAllBytesAsync(defaultProfileImagePath) };
                var newUser = new UserGameVibes {
                    UserName = "mod",
                    Email = "mod@mod.com",
                    EmailConfirmed = true,
                    ProfilePicture = new ProfilePicture { ImageData = await File.ReadAllBytesAsync(defaultProfileImagePath) }
                };
                string userPWD = "Test123.";

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

                HashSet<UserGameVibes> newNotCreatedUsersGameVibes = [
                    new UserGameVibes {
                        UserName = "test",
                        Email = "test@test.com",
                        EmailConfirmed = true,
                        Description = "Hello, I jestem GOD of the INNYCH GRACZY",
                        ProfilePicture = new ProfilePicture { ImageData = await File.ReadAllBytesAsync(defaultProfileImagePath) }
                    }
                ];

                for (int i = 2; i <= 20; i++) {
                    string randomUsername = $"test{i}";

                    if (DbInitializerData.RandomUsernames.Count > 0) {
                        randomUsername = DbInitializerData.RandomUsernames[random.Next(DbInitializerData.RandomUsernames.Count)];
                        DbInitializerData.RandomUsernames.Remove(randomUsername);
                    }

                    var newUserGameVibes = new UserGameVibes {
                        UserName = randomUsername,
                        Email = $"test{i}@test.com",
                        EmailConfirmed = true,
                        Description = "Hello, I'm good player. Test description",
                        ProfilePicture = new ProfilePicture { ImageData = await File.ReadAllBytesAsync(defaultProfileImagePath) }
                    };

                    try {
                        string randomColorHex = new Random().Next(0, 16777215).ToString("X");
                        var defaultProfilePictureUrl = $"https://ui-avatars.com/api/?background={randomColorHex}&bold=true&size=128&color=fff&name=" + newUserGameVibes.UserName;
                        var defaultProfilePicture = await httpClient.GetAsync(defaultProfilePictureUrl, CancellationToken.None);
                        var profilePictureBlob = await defaultProfilePicture.Content.ReadAsByteArrayAsync();

                        newUserGameVibes.ProfilePicture = new ProfilePicture { ImageData = profilePictureBlob, ImageFormat = "image/png" };
                    }
                    catch {
                        newUserGameVibes.ProfilePicture = new ProfilePicture { ImageData = await File.ReadAllBytesAsync(defaultProfileImagePath) };
                        Console.WriteLine("Error while downloading profile picture, set default picture");
                    }

                    newNotCreatedUsersGameVibes.Add(newUserGameVibes);
                }

                string userPWD = "Test123.";

                foreach (var user in newNotCreatedUsersGameVibes) {
                    IdentityResult chkUser = await userManager.CreateAsync(user, userPWD);

                    if (chkUser.Succeeded) {
                        await userManager.AddToRoleAsync(user, "user");
                    }
                }

                foreach (var user in newNotCreatedUsersGameVibes) {
                    testUsers.Add(await userManager.FindByEmailAsync(user.Email!));
                }
            }

            Console.WriteLine($" {(DateTime.Now - startTime).TotalSeconds}");
            startTime = DateTime.Now;
            Console.WriteLine("Adding friends");

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


            Console.WriteLine($" {(DateTime.Now - startTime).TotalSeconds}");
            startTime = DateTime.Now;
            Console.WriteLine("Adding games");
            HashSet<int> steamGameIds = [
                20900, // The Witcher 1
                20920, // The Witcher 2
                292030, // The Witcher 3
                1593500, // God of war 1
                2322010, // God of war 2
                1222670, // The Sims™ 4
                47890, // The Sims™ 3
                256321, // LEGO MARVEL Super Heroes DLC: Asgard Pack
                231430, // Company of Heroes 2
                3035570, // Assassin's Creed Mirage
                934700, //Dead Island 2
                2161700, // Persona 3 Reload
                39510, // Gothic II: Gold Classic
                2679460, // Metaphor: ReFantazio
                460950, // Katana zero
                692850, // Bloodstained: Ritual of the Night
                550, // Left 4 dead 2
                220, // Half-Life 2
                440, // Team Fortress 2
                730, // Counter-Strike 2
                570, // Dota 2
                8930, // Sid Meier's Civilization® V
                219640, // Chivalry: Medieval Warfare
                245620, // Tropico 5
                43110, // Metro 2033
                105600, // Terraria
                736260, // Baba is you
		        253230, // A Hat in Time
		        1145360, // Hades
		        1790600, // Dragon Ball: Sparking! ZERO
		        927380, // Yakuza Kiwami 2
		        1687950, // Persona 5 Royal
		        567640, // Danganronpa V3: Killing Harmony
		        1533420, // Neon White
		        431240, // Golf With Your Friends
		        803600, // Disgaea 5 Complete
		        787480, // Phoenix Wright: Ace Attorney Trilogy
		        587620, // Okami HD
                2378900, // The Coffin of Andy and Leyley
                837470, // Untitled Goose Game
                534380, // Dying Light 2: Reloaded Edition
                1113000, // Persona 4 Golden
                1329500, // SpiderHeck
                1150690, // OMORI
                814380, // Sekiro™: Shadows Die Twice
                3151670, // Trombone Champ: Unflattened
                213670, // South Park™: The Stick of Truth™
                488790, // South Park The Fractured But Whole
                367520, // Hollow Knight
                851850, // DRAGON BALL Z: KAKAROT
                252950, // Rocket League
                1139900, // Ghostrunner
                391720, // Layers of Fear (2016)
                412020, // Metro Exodus
                1370140, // Kao the Kangaroo
                349040, // NARUTO SHIPPUDEN: Ultimate Ninja STORM 4
                213610, // Sonic Adventure™ 2 
                1920480, // River City Girls 2
                1091500, // Cyberpunk 2077
                1158850, // The Great Ace Attorney Chronicles
                // upcoming games:
                3191990, // Tiny House Simulator 2024-11-05
                2651280, // Marvel's Spider-Man 2
                2246340, // Monster Hunter Wilds
                1850050, // Alien: Rogue Incursion
                2671160, // Galactic Civilizations IV - Megastructures
                2677660, // Indiana Jones i Wielki Krąg
                2767030, // Marvel Rivals
            ];

            var createdGames = new List<(Game? game, bool isSuccess)>();

            createdGames.AddRange(await gameService.InitGamesBySteamIdsAsync(applicationDbContext, steamGameIds));

            Console.WriteLine($" {(DateTime.Now - startTime).TotalSeconds}");
            startTime = DateTime.Now;
            Console.WriteLine("Adding 1000 reviews for games");

            for (int i = 0; i < 1000; i++) {
                await reviewService.AddReviewAsync(new Review {
                    GameId = createdGames[random.Next(0, createdGames.Count)].game!.Id,
                    Comment = DbInitializerData.ReviewContent[random.Next(0, DbInitializerData.ReviewContent.Length)],
                    GeneralScore = random.Next(1, 11), // 1-10
                    GraphicsScore = random.Next(1, 11),
                    AudioScore = random.Next(1, 11),
                    GameplayScore = random.Next(1, 11),
                    UserGameVibesId = testUsers[random.Next(testUsers.Count)]!.Id
                });
                Console.Write(".");
            }


            Console.WriteLine($" {(DateTime.Now - startTime).TotalSeconds}");
            startTime = DateTime.Now;
            Console.WriteLine("Adding 50 threads with posts");
            for (int i = 0; i < 50; i++) {
                var newForumThread = await threadService!.AddThreadAsync(testUsers[random.Next(testUsers.Count)]!.Id!,
                    new NewForumThreadDTO {
                        Title = DbInitializerData.ForumThreadsTitles[random.Next(DbInitializerData.ForumThreadsTitles.Length)],
                        SectionId = random.Next(1, 5),
                        FirstForumPostContent = DbInitializerData.PostContent[random.Next(DbInitializerData.PostContent.Length)]
                    }) as dynamic;

                if (newForumThread != null) {
                    int postsCount = random.Next(10, 50);
                    for (int j = 0; j < postsCount; j++) {
                        await postService!.AddForumPostAsync(new ForumPostDTO {
                            Content = DbInitializerData.PostContent[random.Next(DbInitializerData.PostContent.Length)],
                            ThreadId = newForumThread.thread.Id,
                            UserOwnerId = testUsers[random.Next(testUsers.Count)]!.Id
                        });
                    }
                    Console.Write(".");
                }
            }

            Console.WriteLine($" {(DateTime.Now - startTime).TotalSeconds}");
            startTime = DateTime.Now;
            Console.WriteLine("Adding 20 reports for reviews and posts");
            var reviews = await applicationDbContext.Reviews.ToListAsync();
            var forumPosts = await applicationDbContext.ForumPosts.ToListAsync();

            for (int i = 0; i < 20; i++) {
                await postService!.ReportPostAsync(testUsers[random.Next(testUsers.Count)]!.Id!,
                    new ReportPostDTO {
                        ForumPostId = forumPosts[random.Next(0, forumPosts.Count)].Id,
                        Reason = DbInitializerData.SpamSuspicionContent[random.Next(0, DbInitializerData.SpamSuspicionContent.Length)]
                    });

                await reviewService.ReportReviewAsync(testUsers[random.Next(testUsers.Count)]!.Id!,
                    new ReportReviewDTO {
                        ReviewId = reviews[random.Next(0, reviews.Count)].Id,
                        Reason = DbInitializerData.SpamSuspicionContent[random.Next(0, DbInitializerData.SpamSuspicionContent.Length)]
                    });

                Console.Write(".");
            }


            Console.WriteLine($" {(DateTime.Now - startTime).TotalSeconds}");
            startTime = DateTime.Now;
            Console.WriteLine("Adding 2000 likes and 100 dislikes for posts");

            List<int> forumPostsIdsCopy = forumPosts.Select(x => x.Id).ToList();

            for (int i = 0; i < 2000; i++) {
                int idRandomPost = forumPostsIdsCopy[random.Next(0, forumPostsIdsCopy.Count)];

                await postService!.InteractPostAsync(testUsers[random.Next(testUsers.Count)]!.Id!,
                    idRandomPost, true);

                Console.Write(".");
            }

            for (int i = 0; i < 100; i++) {
                int idRandomPost = forumPostsIdsCopy[random.Next(0, forumPostsIdsCopy.Count)];

                await postService!.InteractPostAsync(testUsers[random.Next(testUsers.Count)]!.Id!,
                    idRandomPost, false);

                Console.Write(".");
            }

            Console.WriteLine("\nFinished Init DB");
        }
        catch (Exception e) {
            Console.WriteLine("Error on init DB: " + e.Message);
        }
    }
}

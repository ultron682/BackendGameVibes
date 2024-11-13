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
    private static readonly string[] LoremIpsumWords = {
    "lorem", "ipsum", "dolor", "sit", "amet", "consectetur",
    "adipiscing", "elit", "sed", "do", "eiusmod", "tempor", "incididunt",
    "ut", "labore", "et", "dolore", "magna", "aliqua"
   };

    private static readonly string[] ForumThreadsTitles = new[]
    {
        "Czy ktoś już grał w najnowszą część serii i może podzielić się opinią?",
        "Jakie są wasze ulubione gry na odprężenie po ciężkim dniu?",
        "Mam problem z uruchomieniem gry po ostatniej aktualizacji. Ktoś miał podobny problem?",
        "Ktoś chętny na wspólne granie w ten weekend?",
        "Czy macie jakieś porady dla początkującego gracza?",
        "Jakie gry najbardziej wciągnęły was w ostatnich miesiącach?",
        "Jaka jest wasza ulubiona klasa postaci w grach RPG i dlaczego?",
        "Czy ktoś ma sprawdzone sposoby na optymalizację FPS w tej grze?",
        "Jakie dodatki polecacie do tej gry?",
        "Czy wiecie coś o nadchodzących aktualizacjach?",
        "Co sądzicie o mikrotransakcjach w tej grze?",
        "Macie jakieś doświadczenia z trybem kooperacji? Czy warto zagrać z kimś?",
        "Szukam ekipy do gry wieloosobowej, ktoś chętny?",
        "Która gra według was ma najlepszą fabułę?",
        "Czy opłaca się kupować rozszerzenie do tej gry, czy lepiej poczekać?",
        "Jakie gry polecacie na konsole nowej generacji?",
        "Z jakich postaci lub umiejętności najczęściej korzystacie?",
        "Jakie są wasze ulubione bronie lub zestawy w tej grze?",
        "Słyszeliście o nadchodzącej wersji VR tej gry?",
        "Która gra ma według was najbardziej satysfakcjonujący system walki?"
    };


    private static readonly string[] PostContent = new[]
    {
        "Grafika w tej grze jest niesamowita, naprawdę przyciąga wzrok.",
        "System walki w tej grze jest bardzo płynny i daje dużo satysfakcji.",
        "Uwielbiam eksplorować otwarty świat pełen sekretów i ukrytych lokacji.",
        "Postacie w tej grze są dobrze napisane i łatwo się z nimi utożsamić.",
        "Muzyka w tle dodaje klimatu i sprawia, że gra jest bardziej wciągająca.",
        "Misje poboczne są równie interesujące jak główna fabuła.",
        "Gra ma świetny balans między trudnością a przyjemnością z rozgrywki.",
        "Animacje postaci są bardzo realistyczne i dodają głębi każdej akcji.",
        "Klimat tej gry jest mroczny, ale wciąga i nie pozwala odejść.",
        "Fabuła jest bardzo złożona i pełna niespodziewanych zwrotów akcji.",
        "System rozwoju postaci jest dobrze przemyślany i daje dużo możliwości.",
        "Tryb wieloosobowy to świetna zabawa, szczególnie z przyjaciółmi.",
        "Pogoda i cykl dnia i nocy wpływają na gameplay, co jest ciekawym dodatkiem.",
        "Zadania w grze są różnorodne i nigdy się nie nudzą.",
        "Gra ma wiele zakończeń, co zachęca do jej przejścia kilka razy.",
        "Klimatyczne lokacje sprawiają, że czuję się, jakbym tam naprawdę był.",
        "Interfejs użytkownika jest przejrzysty i łatwy do opanowania.",
        "Ostatnia aktualizacja wprowadziła wiele nowych funkcji i usprawnień.",
        "Gra jest pełna ukrytych przedmiotów, które dodają głębi rozgrywce.",
        "Rozwój bohatera zależy od naszych wyborów, co daje poczucie wolności.",
        "Gra została zoptymalizowana i działa płynnie na moim sprzęcie.",
        "System craftingu jest rozbudowany i satysfakcjonujący.",
        "Każdy region w grze ma swój unikalny klimat i charakter.",
        "Odwiedzane lokacje są zróżnicowane i zapadają w pamięć.",
        "Postacie niezależne mają swoje unikalne historie i osobowości.",
        "Walka z bossami wymaga strategii i dobrego przygotowania.",
        "Broń i ekwipunek można modyfikować, co urozmaica rozgrywkę.",
        "Odkrywanie sekretów i znajdziek to jedna z największych przyjemności w grze.",
        "System punktów doświadczenia pozwala na rozwijanie różnych umiejętności.",
        "Gra zachęca do eksploracji i wynagradza za ciekawość gracza."
    };

    private static readonly string[] SpamSuspicionContent = new[]
    {
        "Ten post wygląda na spam – brak konkretnej treści.",
        "To konto publikowało podobne wpisy, może to być spam.",
        "Ciągle te same linki, wygląda to podejrzanie.",
        "Znów widzę te same wiadomości od tego użytkownika – możliwy spam.",
        "Treść posta nie ma związku z tematem, wygląda to na spam.",
        "Niepokojące, jak często ten użytkownik promuje tę samą stronę.",
        "Brak sensu w wypowiedzi – może to być automatyczny spam.",
        "Post składa się głównie z linków, raczej to nie jest przypadkowe.",
        "Podejrzanie podobne posty widziałem już wcześniej.",
        "Znów to samo kopiowane w różnych wątkach – możliwe, że to bot.",
        "Zawartość posta jest ogólna i nie wnosi nic do dyskusji, wygląda na spam.",
        "Linki w tej wiadomości prowadzą do podejrzanych stron.",
        "To konto tylko reklamuje, nic wartościowego nie wnosi do forum.",
        "Wydaje się, że to konto istnieje tylko po to, by spamować.",
        "Ten wpis powtarza się w wielu wątkach – wygląda na spam.",
        "Treść posta wygląda na wygenerowaną automatycznie.",
        "Wpis jest całkowicie nie na temat – może to być spam.",
        "To kolejna reklama tego samego produktu, możliwe, że to spam.",
        "Ciężko uwierzyć, że to prawdziwy użytkownik – wygląda jak bot.",
        "Brak jakiejkolwiek interakcji z innymi – wygląda na spamowe konto.",
        "Konto zostało stworzone niedawno i od razu reklamuje produkty.",
        "Powtarzające się wzorce wypowiedzi, bardzo przypomina spam.",
        "Jeden post na temat, a reszta to reklamy – raczej to nie przypadek.",
        "Kolejny raz widzę podobne linki, ewidentnie to spam.",
        "Wpisy użytkownika są bardzo schematyczne i bez wartości merytorycznej.",
        "Znów to samo konto promuje te same strony w różnych wątkach.",
        "Treść posta jest chaotyczna i nie wnosi nic wartościowego.",
        "Wszystkie posty tego użytkownika to praktycznie reklamy.",
        "Wygląda na to, że ten użytkownik spamuje różnymi linkami.",
        "Kolejny raz promowanie tej samej strony, wygląda to na spam."
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

            for (int i = 2; i <= 10; i++) {
                var newProfilePicture = new ProfilePicture { ImageData = await File.ReadAllBytesAsync(defaultProfileImagePath) };

                newNotCreatedUsersGameVibes.Add(new UserGameVibes {
                    UserName = $"test{i}",
                    Email = $"test{i}@test.com",
                    EmailConfirmed = true,
                    Description = "Hello, I'm good player. Test description",
                    ProfilePicture = newProfilePicture
                });
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

        var createdGames = new List<(Game? game, bool isSuccess)>();

        createdGames.AddRange(await gameService.InitGamesBySteamIdsAsync(applicationDbContext, steamGameIds));

        Console.WriteLine($" {(DateTime.Now - startTime).TotalSeconds}");
        startTime = DateTime.Now;
        Console.WriteLine("Adding reviews for games");

        for (int i = 0; i < 250; i++) {
            await reviewService.AddReviewAsync(new Review {
                GameId = createdGames[random.Next(0, createdGames.Count)].game!.Id,
                Comment = PostContent[random.Next(0, PostContent.Length)],
                GeneralScore = random.Next(1, 11), // 1-10
                GraphicsScore = random.Next(1, 11),
                AudioScore = random.Next(1, 11),
                GameplayScore = random.Next(1, 11),
                UserGameVibesId = testUsers[random.Next(0, testUsers.Count)]!.Id
            });
            Console.Write(".");
        }


        Console.WriteLine($" {(DateTime.Now - startTime).TotalSeconds}");
        startTime = DateTime.Now;
        Console.WriteLine("Adding threads with posts");
        for (int i = 0; i < 50; i++) {
            var newForumThread = await threadService!.AddThreadAsync(testUsers[random.Next(testUsers.Count)]!.Id!,
                new NewForumThreadDTO {
                    Title = ForumThreadsTitles[random.Next(ForumThreadsTitles.Length)],
                    SectionId = random.Next(1, 5),
                    FirstForumPostContent = PostContent[random.Next(PostContent.Length)]
                }) as dynamic;

            if (newForumThread != null) {
                int postsCount = random.Next(10, 30);
                for (int j = 0; j < postsCount; j++) {
                    await postService!.AddForumPostAsync(new ForumPostDTO {
                        Content = PostContent[random.Next(PostContent.Length)],
                        ThreadId = newForumThread.thread.Id,
                        UserOwnerId = testUsers[random.Next(testUsers.Count)]!.Id
                    });
                }
                Console.Write(".");
            }
        }

        Console.WriteLine($" {(DateTime.Now - startTime).TotalSeconds}");
        startTime = DateTime.Now;
        Console.WriteLine("Adding reports for reviews and posts");
        var reviews = await applicationDbContext.Reviews.ToListAsync();
        var forumPosts = await applicationDbContext.ForumPosts.ToListAsync();

        for (int i = 0; i < 20; i++) {
            await postService!.ReportPostAsync(testUsers[random.Next(0, testUsers.Count)]!.Id!,
                new ReportPostDTO {
                    ForumPostId = forumPosts[random.Next(0, forumPosts.Count)].Id,
                    Reason = SpamSuspicionContent[random.Next(0, SpamSuspicionContent.Length)]
                });

            await reviewService.ReportReviewAsync(testUsers[random.Next(0, testUsers.Count)]!.Id!,
                new ReportReviewDTO {
                    ReviewId = reviews[random.Next(0, reviews.Count)].Id,
                    Reason = SpamSuspicionContent[random.Next(0, SpamSuspicionContent.Length)]
                });

            Console.Write(".");
        }


        Console.WriteLine($" {(DateTime.Now - startTime).TotalSeconds}");
        startTime = DateTime.Now;
        Console.WriteLine("Adding likes and dislikes for posts");

        for (int i = 0; i < 300; i++) {
            await postService!.InteractPostAsync(testUsers[random.Next(0, testUsers.Count)]!.Id!,
                forumPosts[random.Next(0, forumPosts.Count)].Id, true);

            Console.Write(".");
        }

        Console.Write("  ^  ");

        for (int i = 0; i < 100; i++) {
            await postService!.InteractPostAsync(testUsers[random.Next(0, testUsers.Count)]!.Id!,
                forumPosts[random.Next(0, forumPosts.Count)].Id, false);

            Console.Write(".");
        }


        Console.WriteLine("\nFinished Init DB");
    }
}

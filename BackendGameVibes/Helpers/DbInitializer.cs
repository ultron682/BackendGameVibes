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
    private static readonly List<string> RandomUsernames = [
        "TechGuru",
        "ShadowHunter",
        "PixelMaster",
        "CodeCracker",
        "DevChampion",
        "GameNinja",
        "PixelWizard",
        "CyberRogue",
        "CodePhoenix",
        "DarkKnightX",
        "GlitchMancer",
        "QuantumCoder",
        "ZeroX",
        "PixelPanda",
        "NightWolf",
        "EpicCoder",
        "SilentStorm",
        "CodeViper",
        "SkyWalker42",
        "BinaryBeast"
    ];

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

    private static readonly string[] ReviewContent =
    [
        "Grafika w tej grze jest po prostu zachwycająca. Detale otoczenia, animacje i oświetlenie są na najwyższym poziomie. Każda lokacja wygląda, jakby została stworzona z ogromną dbałością o szczegóły. Podczas eksploracji świata gry można dostrzec mnóstwo ukrytych elementów, które cieszą oko. Gra działa płynnie nawet na starszym sprzęcie, co jest dużym plusem. Twórcy zadbali o optymalizację, dzięki czemu doświadczenie z gry jest niezakłócone.",
        "System walki w tej grze to mistrzostwo. Każda potyczka wymaga myślenia i szybkich reakcji. Możliwość łączenia różnych ataków w kombinacje sprawia, że każda walka jest unikalna. Umiejętności postaci są różnorodne i dają wiele możliwości eksperymentowania. Przeciwnicy różnią się zachowaniem, co wymusza zmianę strategii. Dzięki temu gra nie staje się monotonna, a każda walka jest wyzwaniem.",
        "Otwarty świat tej gry to coś niesamowitego. Każdy zakątek kryje tajemnice i sekrety do odkrycia. Świat jest pełen życia, z dynamicznymi wydarzeniami, które dzieją się wokół gracza. Twórcy zadbali, by każde miejsce miało unikalny klimat i historię. Eksploracja jest nie tylko przyjemnością, ale również nagradzana jest znajdźkami i ukrytymi przedmiotami.",
        "Postacie w tej grze są jednymi z najlepiej napisanych, jakie widziałem. Ich dialogi są naturalne i pełne emocji. Każda postać ma własną historię, którą można poznać w trakcie gry. Relacje między postaciami są wiarygodne i rozwijają się w ciekawy sposób. Dzięki temu łatwo się z nimi utożsamić, a ich losy naprawdę obchodzą gracza.",
        "Muzyka w tej grze jest piękna i idealnie dopasowana do klimatu. Ścieżka dźwiękowa zmienia się w zależności od sytuacji, budując napięcie lub wprowadzając nastrój relaksu. Motywy przewodnie łatwo zapadają w pamięć i nadają grze charakteru. Dźwięki otoczenia, takie jak szum lasu czy odgłosy miasta, są równie imponujące i wciągają gracza jeszcze głębiej w świat gry.",
        "Misje poboczne w tej grze to coś więcej niż zwykłe zadania. Każda z nich ma własną historię, która potrafi być równie wciągająca jak główna fabuła. Twórcy zadbali o różnorodność wyzwań, dzięki czemu nie ma poczucia powtarzalności. Niektóre misje odkrywają nowe lokacje lub postacie, co zachęca do ich wykonywania. Nagrody za misje poboczne są wartościowe i naprawdę warte wysiłku.",
        "Balans trudności w tej grze jest idealny. Początkowe etapy pozwalają graczowi oswoić się z mechaniką, ale później gra staje się coraz bardziej wymagająca. Każda walka i każda łamigłówka wymagają odpowiedniego przygotowania. Sukces jest bardzo satysfakcjonujący i daje poczucie osiągnięcia czegoś wyjątkowego. Gra nie jest ani za łatwa, ani zbyt frustrująca.",
        "Animacje w tej grze są na najwyższym poziomie. Postacie poruszają się płynnie i naturalnie, co dodaje realizmu. Nawet najmniejsze detale, takie jak ruchy włosów czy mimika twarzy, są dopracowane. Dzięki temu każda scena wygląda jak żywa. To sprawia, że gra ogląda się jak film.",
        "Klimat gry jest niesamowicie wciągający. Mroczna atmosfera świata sprawia, że chce się dowiedzieć więcej o jego historii. Nawet gdy gra staje się trudna, klimat zachęca do dalszej gry. Wszystko, od projektów lokacji po muzykę i fabułę, tworzy spójną i przyciągającą całość.",
        "Fabuła w tej grze jest jak dobra książka – nie mogłem się oderwać. Jest pełna zwrotów akcji i tajemnic do odkrycia. Każda decyzja, jaką podejmuje gracz, ma wpływ na rozwój historii. Dzięki temu mam poczucie, że naprawdę uczestniczę w tworzeniu tej opowieści. Zakończenia są różnorodne i zachęcają do wielokrotnego przechodzenia gry.",
        "System rozwoju postaci w tej grze daje ogromną swobodę. Mogę wybierać umiejętności, które najlepiej pasują do mojego stylu gry. Nie ma jedynej słusznej drogi, co dodaje głębi rozgrywce. Możliwość eksperymentowania z różnymi stylami gry sprawia, że każda sesja jest wyjątkowa. Każdy poziom doświadczenia daje poczucie realnego postępu.",
        "Tryb wieloosobowy to coś, co wyróżnia tę grę. Grając z przyjaciółmi, zabawa wchodzi na zupełnie nowy poziom. Wspólne eksplorowanie świata gry czy rozwiązywanie zagadek to świetne doświadczenie. Możliwość rywalizacji w trybie PvP również daje dużo emocji. Serwery działają stabilnie, co zapewnia płynną rozgrywkę.",
        "Pogoda i cykl dnia oraz nocy w tej grze są imponujące. Każda zmiana warunków atmosferycznych wpływa na gameplay, co dodaje realizmu. W nocy przeciwnicy są trudniejsi, co zachęca do bardziej ostrożnej gry. Dodatkowo oprawa wizualna podczas burzy czy zachodu słońca zapiera dech w piersiach.",
        "Zadania w grze są naprawdę różnorodne i interesujące. Oprócz głównych misji, mamy mnóstwo pobocznych aktywności. Każde zadanie wprowadza coś nowego, co utrzymuje gracza w ciągłym zainteresowaniu. Warto poświęcić czas na eksplorację i wykonywanie dodatkowych misji.",
        "Możliwość odblokowania wielu zakończeń jest jednym z największych atutów tej gry. Każda decyzja, jaką podejmuję, ma realne konsekwencje. Dzięki temu gra zachęca do powrotu i odkrycia wszystkich możliwych ścieżek fabularnych. Jest to idealne dla graczy, którzy lubią eksplorować każdą opcję.",
        "Lokacje w tej grze to prawdziwe dzieła sztuki. Każde miejsce opowiada swoją historię i kryje sekrety. Świat gry jest pełen różnorodnych terenów, od gęstych lasów po zatopione miasta. Eksploracja daje niesamowitą satysfakcję i sprawia, że gra jest jeszcze bardziej wciągająca.",
        "Interfejs użytkownika jest intuicyjny i bardzo dobrze zaprojektowany. Nawigacja po menu jest prosta i szybka, co pozwala skupić się na rozgrywce. Wszystkie niezbędne informacje są łatwo dostępne, a dodatkowe opcje personalizacji to duży plus. Gra jest przyjazna zarówno dla nowych, jak i doświadczonych graczy.",
        "Ostatnia aktualizacja wprowadziła wiele nowych funkcji, które znacznie poprawiły grę. Dodano nowe lokacje, bronie i misje, co daje więcej możliwości rozgrywki. Optymalizacja została zauważalnie ulepszona, co pozytywnie wpływa na wydajność. Twórcy naprawdę słuchają społeczności graczy i dostarczają to, czego oczekują.",
        "Ukryte przedmioty i znajdźki w tej grze to coś, co bardzo cenię. Poszukiwanie sekretów to świetna zabawa, a nagrody są zawsze warte wysiłku. Gra zachęca do eksploracji i dokładnego przeszukiwania każdego zakątka. Dzięki temu świat gry wydaje się pełen życia i niespodzianek.",
        "System craftingu w tej grze to prawdziwa perełka. Możliwość tworzenia unikalnych przedmiotów i ulepszania ekwipunku dodaje głębi rozgrywce. Proces tworzenia jest intuicyjny, ale wymaga zdobywania odpowiednich surowców. Dzięki temu crafting nie tylko bawi, ale i nagradza graczy za ich wysiłek."
    ];


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
                    }
                ];

                for (int i = 2; i <= 20; i++) {
                    string randomUsername = $"test{i}";

                    if (RandomUsernames.Count > 0) {
                        randomUsername = RandomUsernames[random.Next(RandomUsernames.Count)];
                        RandomUsernames.Remove(randomUsername);
                    }

                    var newUserGameVibes = new UserGameVibes {
                        UserName = randomUsername,
                        Email = $"test{i}@test.com",
                        EmailConfirmed = true,
                        Description = "Hello, I'm good player. Test description",
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
                // Upcoming games:
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
            Console.WriteLine("Adding reviews for games");

            for (int i = 0; i < 300; i++) {
                await reviewService.AddReviewAsync(new Review {
                    GameId = createdGames[random.Next(0, createdGames.Count)].game!.Id,
                    Comment = ReviewContent[random.Next(0, ReviewContent.Length)],
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
            Console.WriteLine("Adding threads with posts");
            for (int i = 0; i < 50; i++) {
                var newForumThread = await threadService!.AddThreadAsync(testUsers[random.Next(testUsers.Count)]!.Id!,
                    new NewForumThreadDTO {
                        Title = ForumThreadsTitles[random.Next(ForumThreadsTitles.Length)],
                        SectionId = random.Next(1, 5),
                        FirstForumPostContent = PostContent[random.Next(PostContent.Length)]
                    }) as dynamic;

                if (newForumThread != null) {
                    int postsCount = random.Next(10, 50);
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

            for (int i = 0; i < 50; i++) {
                await postService!.InteractPostAsync(testUsers[random.Next(0, testUsers.Count)]!.Id!,
                    forumPosts[random.Next(0, forumPosts.Count)].Id, false);

                Console.Write(".");
            }


            Console.WriteLine("\nFinished Init DB");
    }
}

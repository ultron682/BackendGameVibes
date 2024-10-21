using BackendGameVibes.IServices;
using BackendGameVibes.Models;
using BackendGameVibes.Services;
using Microsoft.AspNetCore.Identity;

namespace BackendGameVibes.Helpers {
    public class DbInitializer {
        public static async Task InitializeAsync(AsyncServiceScope scope) {
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserGameVibes>>();
            using var reviewService = scope.ServiceProvider.GetRequiredService<IReviewService>();
            using var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();
            using var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();

            Console.WriteLine("Start Init DB");

            await roleService!.InitRolesAndUsers();

            UserGameVibes? user = await userManager.FindByEmailAsync("test@test.com");

            Game? createdGame = await gameService.CreateGame(292030); // The Witcher 3
            await gameService.CreateGame(20900); // The Witcher 1
            await gameService.CreateGame(20920); // The Witcher 2

            await gameService.CreateGame(1593500); // God of war 1
            await gameService.CreateGame(2322010); // God of war 2
            await gameService.CreateGame(1222670); // The Sims™ 4
            await gameService.CreateGame(47890); // The Sims™ 3
            await gameService.CreateGame(256321); // LEGO MARVEL Super Heroes DLC: Asgard Pack
            await gameService.CreateGame(231430); // Company of Heroes 2



            if (createdGame != null) {
                await reviewService.AddReviewAsync(new Review {
                    GameId = createdGame!.Id,
                    Comment = "Great game. Review #1 created automatic in Program.cs",
                    GeneralScore = 9.5,
                    GraphicsScore = 7.5,
                    AudioScore = 6.5,
                    GameplayScore = 8.9,
                    UserGameVibesId = user!.Id
                });

                await reviewService.AddReviewAsync(new Review {
                    GameId = createdGame!.Id,
                    Comment = "Great game in my life. Review #2 created automatic in Program.cs",
                    GeneralScore = 9.5,
                    GraphicsScore = 7.5,
                    AudioScore = 6.5,
                    GameplayScore = 8.9,
                    UserGameVibesId = user!.Id
                });
            }

            Console.WriteLine("Finish Init DB");
        }
    }
}

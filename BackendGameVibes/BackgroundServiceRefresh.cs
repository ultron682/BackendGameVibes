using BackendGameVibes.Services;

namespace BackendGameVibes {
    public class BackgroundServiceRefresh : IDisposable, IHostedService {
        private Timer? _timer;
        private readonly SteamService _steamService;

        public BackgroundServiceRefresh(SteamService steamService) {
            _steamService = steamService;
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            _timer = new Timer(RefreshSteamGames, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private void RefreshSteamGames(object? state) {
            _steamService.InitSteamApi();
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() {
            _timer?.Dispose();
        }
    }
}

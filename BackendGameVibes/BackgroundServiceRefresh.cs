namespace BackendGameVibes {
    public class BackgroundServiceRefresh : IDisposable, IHostedService {
        private Timer? _timer;

        public Task StartAsync(CancellationToken cancellationToken) {
            _timer = new Timer(RefreshWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void RefreshWork(object? state) {
            Console.WriteLine("Do roboty moi kopacze złota");
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

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BackendGameVibes.Helpers {
    public class GameVibesHealthInternetCheck : IHealthCheck {

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
            try {
                using (var httpClient = new HttpClient()) {
                    var response = await httpClient.GetAsync("https://1.1.1.1", cancellationToken);
                    if (response.IsSuccessStatusCode) {
                        return HealthCheckResult.Healthy("Internet connection is healthy");
                    }
                    else {
                        return HealthCheckResult.Unhealthy("Internet connection failed");
                    }
                }
            }
            catch (HttpRequestException ex) {
                return HealthCheckResult.Unhealthy("Internet connection failed", ex);
            }
        }
    }
}
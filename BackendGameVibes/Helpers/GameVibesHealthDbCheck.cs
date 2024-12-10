using BackendGameVibes.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BackendGameVibes.Helpers;

public class GameVibesHealthDbCheck : IHealthCheck {
    private readonly ApplicationDbContext _context;
    public GameVibesHealthDbCheck(ApplicationDbContext context) {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        try {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            return HealthCheckResult.Healthy("Database connection is healthy");
        }
        catch (Exception ex) {
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}
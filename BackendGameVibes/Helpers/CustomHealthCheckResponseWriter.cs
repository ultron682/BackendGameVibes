using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace BackendGameVibes.Helpers {
    public static class CustomHealthCheckResponseWriter {
        public static async Task WriteCustomHealthCheckResponse(HttpContext context, HealthReport report) {
            context.Response.ContentType = "application/json";

            var healthCheckResults = new Dictionary<string, object>
            {
                { "status", report.Status.ToString() },
                { "results", new Dictionary<string, object>() }
            };

            foreach (var entry in report.Entries) {
                var entryValues = new Dictionary<string, object>
                {
                    { "status", entry.Value.Status.ToString() },
                    { "description", entry.Value.Description! },
                    { "duration", entry.Value.Duration.ToString() }
                };

                ((Dictionary<string, object>)healthCheckResults["results"]).Add(entry.Key, entryValues);
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(healthCheckResults, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}

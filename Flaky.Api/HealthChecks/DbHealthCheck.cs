using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Flaky.Api.HealthChecks;
public class DbHealthCheck : IHealthCheck
{
    private readonly NpgsqlConnection _db;

    public DbHealthCheck(NpgsqlConnection conn)
    {
        _db = conn;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _db.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT 1", _db);
            await cmd.ExecuteNonQueryAsync();
            
            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(e.Message);
        }
    }
}

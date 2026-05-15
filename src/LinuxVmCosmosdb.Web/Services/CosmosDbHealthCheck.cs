using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LinuxVmCosmosdb.Web.Services;

public class CosmosDbHealthCheck : IHealthCheck
{
    private readonly CosmosClient _client;

    public CosmosDbHealthCheck(CosmosClient client)
    {
        _client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.ReadAccountAsync();
            return HealthCheckResult.Healthy("Cosmos DB connection is healthy.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cosmos DB connection failed.", ex);
        }
    }
}

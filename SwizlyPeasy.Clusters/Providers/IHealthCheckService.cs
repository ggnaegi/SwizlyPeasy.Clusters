namespace SwizlyPeasy.Clusters.Providers;

public interface IHealthCheckService
{
    /// <summary>
    ///     Checking if service's destination is healthy
    ///     (checks are passing)
    /// </summary>
    /// <param name="clusterId"></param>
    /// <param name="destinationId"></param>
    /// <returns></returns>
    public Task<bool> IsDestinationHealthy(string clusterId, string destinationId);
}
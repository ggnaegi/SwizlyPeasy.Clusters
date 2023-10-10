using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Clusters.Services;

public interface IClusterConfigService
{
    /// <summary>
    ///     Retrieving a list of YARP ClusterConfig objects.
    ///     This list is created dynamically, calling the service discovery server,
    ///     creating clusters by grouping service instances by service name.
    /// </summary>
    /// <returns></returns>
    Task<List<ClusterConfig>> RetrieveClustersConfig();
}
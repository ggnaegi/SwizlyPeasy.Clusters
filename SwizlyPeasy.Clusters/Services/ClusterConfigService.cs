using SwizlyPeasy.Clusters.Providers;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Clusters.Services;

public class ClusterConfigService : IClusterConfigService
{
    private readonly IRetrieveDestinationsService _retrieveDestinationsService;

    public ClusterConfigService(IRetrieveDestinationsService retrieveDestinationsService)
    {
        _retrieveDestinationsService = retrieveDestinationsService ?? throw new ArgumentNullException(nameof(retrieveDestinationsService));
    }

    /// <summary>
    /// Retrieving cluster config data from service discovery provider
    /// </summary>
    /// <returns></returns>
    public async Task<List<ClusterConfig>> RetrieveClustersConfig()
    {
        var destCollection = await _retrieveDestinationsService.RetrieveDestinations();

        if (!destCollection.Any())
        {
            return new List<ClusterConfig>();
        }


        return (from currentCollection in destCollection
            select new ClusterConfig
            {
                ClusterId = currentCollection.ServiceName,
                LoadBalancingPolicy = currentCollection.LoadBalancingPolicy,
                Destinations = currentCollection.RegisteredDestinations
                    .Select(x =>
                        (x.Id,
                            new DestinationConfig
                                { Address = $"{currentCollection.HttpScheme}://{x.Address}:{x.Port}" }))
                    .ToDictionary(y => y.Id, y => y.Item2)
            }).ToList();
    }
}
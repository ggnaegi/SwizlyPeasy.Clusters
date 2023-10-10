using Microsoft.Extensions.Options;
using SwizlyPeasy.Clusters.Providers;
using SwizlyPeasy.Common.Dtos;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Clusters.Services;

public class ClusterConfigService : IClusterConfigService
{
    private readonly IRetrieveDestinationsService _agentsService;
    private readonly IOptions<ServiceDiscoveryConfig> _config;

    public ClusterConfigService(IRetrieveDestinationsService agentsService, IOptions<ServiceDiscoveryConfig> config)
    {
        _agentsService = agentsService ?? throw new ArgumentNullException(nameof(agentsService));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task<List<ClusterConfig>> RetrieveClustersConfig()
    {
        var agentsDic = await _agentsService.RetrieveDestinations();

        if (!agentsDic.Any()) return new List<ClusterConfig>();

        return agentsDic.Keys
            .Select(serviceName => new ClusterConfig
            {
                ClusterId = serviceName,
                LoadBalancingPolicy = _config.Value.LoadBalancingPolicy,
                Destinations = agentsDic[serviceName]
                    .Select(x => (x.Id,
                        new DestinationConfig { Address = $"{_config.Value.Scheme}://{x.Address}:{x.Port}" }))
                    .ToDictionary(y => y.Id, y => y.Item2)
            })
            .ToList();
    }
}
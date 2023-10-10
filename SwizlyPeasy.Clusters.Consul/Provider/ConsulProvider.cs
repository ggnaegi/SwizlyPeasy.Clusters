using System.Net;
using Consul;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Clusters.Dtos;
using SwizlyPeasy.Clusters.Providers;
using SwizlyPeasy.Common.Dtos.Status;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Clusters.Consul.Provider;

public class ConsulProvider : IServiceDiscoveryProviderClient
{
    private readonly ConsulClient _consulClient;

    public ConsulProvider(IOptions<SwizlyPeasyConfig> config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        _consulClient = new ConsulClient(x => { x.Address = config.Value.ServiceDiscovery.ServiceDiscoveryAddress; });
    }

    public async Task<bool> IsDestinationHealthy(string clusterId, string destinationId)
    {
        QueryResult<dynamic>? serviceHealth;

        // handling exceptions from consul client
        // consul could throw a ConsulRequestException with status 503
        try
        {
            serviceHealth =
                await _consulClient.Raw.Query($"/v1/agent/health/service/id/{destinationId}", new QueryOptions());
        }
        catch (ConsulRequestException)
        {
            return false;
        }


        if (serviceHealth.StatusCode == HttpStatusCode.NotFound)
        {
            throw new InternalDomainException($"No Health Checks for service with Id {destinationId} can be found.",
                null);
        }

        if (serviceHealth.Response == null)
        {
            throw new InternalDomainException($"Consul returned an empty response for service with Id {destinationId}",
                null);
        }

        HealthEndpointStatusDto status = serviceHealth.Response.ToObject(typeof(HealthEndpointStatusDto));

        return status.AggregatedStatus != null && HealthStatus.Passing.Status == status.AggregatedStatus;
    }

    public async Task SaveToKeyValueStore(string key, byte[] value)
    {
        var kvPair = new KVPair(key)
        {
            Value = value
        };

        var writeResult = await _consulClient.KV.Put(kvPair);
        if (!writeResult.Response)
        {
            throw new InternalDomainException("Unable to save data to key value store...", null);
        }
    }

    public async Task<byte[]> GetFromKeyValueStore(string key)
    {
        var result = await _consulClient.KV.Get(key);

        if (result.StatusCode == HttpStatusCode.NotFound)
        {
            throw new InternalDomainException($"Key: {key} not found in key value store...", null);
        }

        return result.Response.Value;
    }

    public async Task<IList<RegisteredDestinationsCollection>> RetrieveDestinations()
    {
        var queryResult = await _consulClient.Agent.Services();
        var services = queryResult.Response;

        var destinationsCollections = new List<RegisteredDestinationsCollection>();
        if (!services.Any())
        {
            return destinationsCollections;
        }

        foreach (var service in services.Values)
        {
            if (destinationsCollections.All(x => x.ServiceName != service.Service))
            {
                destinationsCollections.Add(new RegisteredDestinationsCollection
                {
                    ServiceName = service.Service,
                    RegisteredDestinations = new List<RegisteredDestination>()
                });
            }

            var currentCollection = destinationsCollections.First(x => x.ServiceName == service.Service);
            currentCollection.RegisteredDestinations.Add(new RegisteredDestination
            {
                Id = service.ID,
                Address = service.Address,
                Port = service.Port
            });
        }

        return destinationsCollections;
    }
}
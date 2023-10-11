using SwizlyPeasy.Clusters.Providers;
using Microsoft.Extensions.Options;
using Steeltoe.Discovery;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Discovery.Eureka.AppInfo;
using SwizlyPeasy.Clusters.Dtos;
using Microsoft.Extensions.Caching.Memory;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Clusters.Eureka.Provider;

public class EurekaProvider : IServiceDiscoveryProviderClient
{
    private readonly DiscoveryClient _eurekaClient;
    private readonly SwizlyPeasyConfig _config;

    // eureka doesn't provide a memory cache or key vault, so we use the one from asp.net core
    private readonly IMemoryCache _memoryCache;

    public EurekaProvider(IMemoryCache memoryCache, IDiscoveryClient eurekaClient, IOptions<SwizlyPeasyConfig> config)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _eurekaClient = (DiscoveryClient)eurekaClient ?? throw new ArgumentNullException(nameof(eurekaClient));
        _config = config.Value ?? throw new ArgumentNullException(nameof(config));
    }

    public Task<bool> IsDestinationHealthy(string clusterId, string destinationId)
    {
        var instanceInfo = _eurekaClient.GetInstanceById(destinationId).FirstOrDefault();

        return instanceInfo == null
            ? Task.FromResult(false)
            : Task.FromResult(instanceInfo.Status == InstanceStatus.UP);
    }

    public Task SaveToKeyValueStore(string key, byte[] value)
    {
        _memoryCache.Set(key, value);

        return Task.CompletedTask;
    }

    public Task<byte[]> GetFromKeyValueStore(string key)
    {
        _memoryCache.TryGetValue(key, out byte[]? value);

        if (value == null)
        {
            throw new InternalDomainException($"No value for key {key} was found in the cache.", null);
        }

        return Task.FromResult(value);
    }

    public Task<IList<RegisteredDestinationsCollection>> RetrieveDestinations()
    {
        var serviceNames = _config.ServiceDiscovery.KnownServicesNames;

        var servicesCollection = (from serviceName in serviceNames
            let instances = _eurekaClient.GetInstanceById(serviceName)
            select new RegisteredDestinationsCollection
            {
                ServiceName = serviceName,
                RegisteredDestinations = instances.Select(instance => new RegisteredDestination
                    { Id = instance.InstanceId, Address = instance.HostName, Port = instance.Port }).ToList()
            }).ToList();

        return Task.FromResult<IList<RegisteredDestinationsCollection>>(servicesCollection);
    }
}
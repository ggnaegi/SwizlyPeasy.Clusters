using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Discovery.Client;

namespace SwizlyPeasy.Clusters.Eureka.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddEurekaClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDiscoveryClient(configuration);
    }
}
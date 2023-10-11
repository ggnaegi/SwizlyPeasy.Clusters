namespace SwizlyPeasy.Clusters.Providers
{
    public interface IServiceDiscoveryProviderClient : IHealthCheckService, IKeyValueService, IRetrieveDestinationsService
    {
    }
}

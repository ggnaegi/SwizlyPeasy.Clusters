using SwizlyPeasy.Clusters.Dtos;

namespace SwizlyPeasy.Clusters.Providers;

public interface IRetrieveDestinationsService
{
    /// <summary>
    ///     Retrieving registered services from consul
    ///     The services are grouped by service name.
    ///     This makes it easier to create the cluster part of the yarp configuration
    /// </summary>
    /// <returns></returns>
    Task<IList<RegisteredDestinationsCollection>> RetrieveDestinations();
}
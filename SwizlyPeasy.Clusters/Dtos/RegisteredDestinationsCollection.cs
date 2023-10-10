namespace SwizlyPeasy.Clusters.Dtos;

public class RegisteredDestinationsCollection
{
    public required IList<RegisteredDestination> RegisteredDestinations { get; set; }
    public string HttpScheme { get; set; } = Uri.UriSchemeHttp;
    public string LoadBalancingPolicy { get; set; } = "Random";
    public required string ServiceName { get; set; }
}
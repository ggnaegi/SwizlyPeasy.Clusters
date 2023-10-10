namespace SwizlyPeasy.Clusters.Providers;

public class RegisteredDestination
{
    public required string Id { get; set; }
    public required string Service { get; set; }
    public int Port { get; set; }
    public required string Address { get; set; }
}
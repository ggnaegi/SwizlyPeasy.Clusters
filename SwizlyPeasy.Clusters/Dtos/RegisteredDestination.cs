namespace SwizlyPeasy.Clusters.Dtos;

public class RegisteredDestination
{
    public required string Id { get; set; }
    public int Port { get; set; }
    public required string Address { get; set; }
}
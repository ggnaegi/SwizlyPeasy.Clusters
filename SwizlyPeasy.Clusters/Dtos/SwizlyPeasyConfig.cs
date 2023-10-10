using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Clusters.Dtos;

public class SwizlyPeasyConfig
{
    public required ServiceDiscoveryConfig ServiceDiscovery { get; set; }
    public string RouteConfigFilePath { get; set; } = "routes.config.json";
}
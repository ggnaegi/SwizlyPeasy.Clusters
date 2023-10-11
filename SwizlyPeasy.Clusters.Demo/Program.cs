using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using SwizlyPeasy.Clusters.Consul.Provider;
using SwizlyPeasy.Clusters.Eureka.Extensions;
using SwizlyPeasy.Clusters.Eureka.Provider;
using SwizlyPeasy.Clusters.Extensions;
using SwizlyPeasy.Common.Extensions;
using SwizlyPeasy.Common.Middlewares;

var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddControllers();
    builder.Services.AddHttpClient();
    if (builder.Environment.EnvironmentName is "Development")
    {
        builder.Services
            .AddReverseProxy()
            .LoadFromServiceDiscoveryProvider<ConsulProvider>(builder.Configuration);
    }
    else
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddEurekaClient(builder.Configuration);
        builder.Services
            .AddReverseProxy()
            .LoadFromServiceDiscoveryProvider<EurekaProvider>(builder.Configuration);
    }

    builder.Host.UseNLog();

    var app = builder.Build();

    app.UseKnownProxiesAndNetworks();
    app.UseMiddleware<ExceptionsHandlerMiddleware>();
    app.Use404AsException();
    app.UseRouting();
    app.MapControllers();
    app.MapReverseProxy();

    app.Run();
}
catch (Exception e)
{
    logger.Error(e, "Stopped program because of exception");
}
finally
{
    LogManager.Shutdown();
}
using SwizlyPeasy.Common.Extensions;
using SwizlyPeasy.Common.HealthChecks;
using SwizlyPeasy.Consul.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// swizly peasy consul & health checks
builder.Services.RegisterServiceToSwizlyPeasyGateway(builder.Configuration);

var app = builder.Build();
app.UseSwizlyPeasyExceptions();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
//--------- Swizly Peasy MiddleWares ----------
// swizly peasy health checks middleware
app.UseSwizlyPeasyHealthChecks();
//---------------------------------------------
app.UseAuthorization();
app.MapControllers();

app.Run();


namespace SwizlyPeasy.Clusters.Demo.Client
{
    /// <summary>
    ///     For Integration Tests...
    /// </summary>
    public partial class Program
    {
    }
}
using EndpointTranslator.Abstractions.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EndpointTranslator.Plugins.Logging.Serilog;

internal class SerilogPlugin : IPlugin
{
    public Task StartupAsync(
        IHostBuilder builder,
        IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        
        Log.Information("Setting up Serilog...");
        
        builder.UseSerilog((context, cfg) =>
            cfg.ReadFrom.Configuration(context.Configuration));

        return Task.CompletedTask;
    }

    public Task ConfigureAsync(IApplicationBuilder builder)
    {
        builder.UseSerilogRequestLogging();

        return Task.CompletedTask;
    }
}
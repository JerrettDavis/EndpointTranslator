using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EndpointTranslator.Abstractions.Plugins;

public interface IPlugin
{
    Task StartupAsync(
        IHostBuilder builder,
        IServiceCollection services, 
        IConfiguration configuration);

    Task ConfigureAsync(IApplicationBuilder builder);
}
using EndpointTranslator.Abstractions.Plugins;
using EndpointTranslator.Abstractions.Translators;
using EndpointTranslator.Translators;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EndpointTranslator.Plugins.Translators.JsonTemplateTranslator;

public class TranslatorStartupRegistration : IPlugin
{
    public Task StartupAsync(
        IHostBuilder builder, 
        IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddTransient<ITranslator<string, JsonTemplateTranslatorParameters, string>, JsonTemplateTranslator>()
            .AddTransient<ITranslatorFactory, JsonTemplateTranslatorFactory>();

        return Task.CompletedTask;
    }

    public Task ConfigureAsync(IApplicationBuilder builder)
    {
        return Task.CompletedTask;
    }
}
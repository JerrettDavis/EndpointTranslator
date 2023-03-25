namespace EndpointTranslator.Common.Extensions.Startup;

public static class ConfigurationExtensions
{
    public static T Parse<T>(this IConfiguration configuration)
    {
        var settings = Activator.CreateInstance<T>();
        configuration.Bind(settings);
        return settings;
    }
}
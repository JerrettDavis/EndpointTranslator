using System.IO.Abstractions;
using System.Reflection;
using EndpointTranslator.Abstractions.Plugins;
using EndpointTranslator.Models;
using EndpointTranslator.Plugins;

namespace EndpointTranslator.Common.Extensions.Startup;

public static class PluginStartupExtensions
{
    public static IAsyncEnumerable<IPlugin> LoadPluginsAsync(this WebApplicationBuilder builder)
    {
        using var loggerFactory = LoggerFactory.Create(b =>
        {
            b.SetMinimumLevel(LogLevel.Information);
            b.AddConsole();
            b.AddConfiguration(builder.Configuration);
            b.AddEventSourceLogger();
        });
        var settings = builder.Configuration.Parse<AppSettings>();
        var logger = loggerFactory.CreateLogger<PluginFileProvider>();
        var fileSystem = new FileSystem();
        var pluginFileProvider = new PluginFileProvider(logger, fileSystem);
        var pluginAssemblies = pluginFileProvider.GetPluginsAsync(settings.Plugins.Directory);

        return pluginAssemblies
            .SelectMany(path => LoadPluginAssembly(path, fileSystem)
                .CreatePlugins<IPlugin>()
                .ToAsyncEnumerable())
            .Where(p => p != null)!;
    }
    
    private static Assembly LoadPluginAssembly(string path, IFileSystem fileSystem)
    {
        path = fileSystem.Path.GetFullPath(path.Replace('\\', fileSystem.Path.DirectorySeparatorChar));
        Console.WriteLine($"Loading plugin from: {path}");
        var loadContext = new PluginLoadContext(path);
        return loadContext.LoadFromAssemblyName(
            new AssemblyName(fileSystem.Path.GetFileNameWithoutExtension(path)));
    }

    private static IEnumerable<T?> CreatePlugins<T>(this Assembly assembly)
    {
        var assemblies = assembly.GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<T>()
            .Where(type => type != null)
            .ToList();

        if (assemblies.Any()) 
            return assemblies;

        var availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
        throw new ApplicationException(
            $"Can't find any type which implements IPlugin in {assembly} from {assembly.Location}.\n" +
            $"Available types: {availableTypes}");
    }
}
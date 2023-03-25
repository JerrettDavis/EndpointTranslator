namespace EndpointTranslator.Plugins;

public interface IPluginFileProvider
{
    IAsyncEnumerable<string> GetPluginsAsync(
        string pluginDirectory,
        CancellationToken cancellationToken = default);
}
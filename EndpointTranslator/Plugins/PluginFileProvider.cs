using System.IO.Abstractions;
using System.Text.Json;
using EndpointTranslator.Models.Plugins;

namespace EndpointTranslator.Plugins;

public class PluginFileProvider : IPluginFileProvider
{
    private const string ManifestFileName = "manifest.json";
    private readonly ILogger<PluginFileProvider> _logger;
    private readonly IFileSystem _fileSystem;

    public PluginFileProvider(
        ILogger<PluginFileProvider> logger,
        IFileSystem fileSystem)
    {
        _logger = logger;
        _fileSystem = fileSystem;
    }

    public IAsyncEnumerable<string> GetPluginsAsync(
        string pluginDirectory,
        CancellationToken cancellationToken = default)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        return _fileSystem.Directory
            .EnumerateDirectories(_fileSystem.Path.Combine(
                basePath,
                pluginDirectory))
            .Select(GetManifestPath)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToAsyncEnumerable()
            .SelectAwait(async p => new
            {
                Path = p, 
                Manifest = await GetManifestAsync(p!, cancellationToken)
            })
            .Where(p => p.Manifest != null)
            .Select(r =>
                _fileSystem.Path.Combine(
                    _fileSystem.Path.GetDirectoryName(r.Path!)!, 
                    $"{r.Manifest!.PluginAssembly}.dll"));
    }

    private string? GetManifestPath(string directory) =>
        _fileSystem.Directory
            .EnumerateFiles(directory)
            .FirstOrDefault(f =>
                _fileSystem.Path.GetFileName(f)
                    .ToLowerInvariant() == ManifestFileName);

    private async Task<Manifest?> GetManifestAsync(
        string path,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = _fileSystem.File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<Manifest>(stream,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading manifest from '{Path}'", path);
            return null;
        }
    }
}
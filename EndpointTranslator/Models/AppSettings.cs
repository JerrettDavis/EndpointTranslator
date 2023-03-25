namespace EndpointTranslator.Models;

public record AppSettings
{
    public PluginsSettings Plugins { get; init; } 
    public IEnumerable<TranslationSettings> Translations { get; init; } 
}

public record PluginsSettings
{
    public string Directory { get; init; } 
}

public record EndpointSettings
{
    public string Name { get; init; } 
    public string Url { get; init; } 
    public HttpMethod Method { get; init; }
}

public record TranslationSettings
{
    public string Name { get; init; }
    public EndpointSettings Inbound { get; init; } 
    public EndpointSettings Outbound { get; init; } 
    public IEnumerable<TranslatorSettings> Translators { get; init; }
    public string Template { get; init; }
}

public record TranslatorSettings
{
    public string Name { get; init; }
    public object Parameters { get; init; }
}
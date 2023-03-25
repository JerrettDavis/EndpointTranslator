using EndpointTranslator.Abstractions.Translators;

namespace EndpointTranslator.Plugins.Translators.JsonTemplateTranslator;

public class JsonTemplateTranslatorParameters : ITranslatorParameters 
{
    public string Template { get; private set; }
    
    public void LoadFromString(string? input)
    {
        Template = input ?? "";
    }

    public void LoadFromObject(object? input)
    {
        Template = input?.ToString() ?? "";
    }
}
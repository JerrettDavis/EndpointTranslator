using EndpointTranslator.Abstractions.Translators;

namespace EndpointTranslator.Plugins.Translators.JsonTemplateTranslator;

public class JsonTemplateTranslatorFactory : ITranslatorFactory
{
    public ITranslator CreateTranslator()
    {
        return new JsonTemplateTranslator();
    }

    public bool AppliesTo(string key) => nameof(JsonTemplateTranslator) == key;
}
using EndpointTranslator.Abstractions.Translators;

namespace EndpointTranslator.Translators;

public class TranslatorStrategy : ITranslatorStrategy
{
    private readonly IEnumerable<ITranslatorFactory> _factories;

    public TranslatorStrategy(IEnumerable<ITranslatorFactory> factories)
    {
        _factories = factories;
    }

    public ITranslator CreateTranslator(string key)
    {
        var factory = _factories.FirstOrDefault(f => f.AppliesTo(key));

        if (factory == null)
            throw new InvalidOperationException($"No factory registered for key {key}");

        return factory.CreateTranslator();
    }
}
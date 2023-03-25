namespace EndpointTranslator.Abstractions.Translators;

public interface ITranslatorStrategy
{
    ITranslator CreateTranslator(string key);
}
namespace EndpointTranslator.Abstractions.Translators;

public interface ITranslatorFactory
{
    ITranslator CreateTranslator();
    
    bool AppliesTo(string key);
}
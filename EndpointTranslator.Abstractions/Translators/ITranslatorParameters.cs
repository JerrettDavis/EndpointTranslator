namespace EndpointTranslator.Abstractions.Translators;

public interface ITranslatorParameters
{
    void LoadFromString(string? input);
    void LoadFromObject(object? input);
}
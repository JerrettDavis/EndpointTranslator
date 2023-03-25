namespace EndpointTranslator.Abstractions.Translators;

public interface ITranslator<in TSource, in TParams, TDestination> : ITranslator
 where TParams : ITranslatorParameters
{
    Task<TDestination> TranslateAsync(
        TSource source, 
        TParams parameters,
        CancellationToken cancellationToken = default);
}

public interface ITranslator
{
    Type GetParametersType();
}
using System.Dynamic;
using System.Xml.Linq;
using EndpointTranslator.Abstractions.Translators;
using EndpointTranslator.Common.Extensions;
using EndpointTranslator.Endpoints.Services;
using EndpointTranslator.Models;
using EndpointTranslator.Translators;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Scriban;
using Scriban.Runtime;
using Endpoint = Microsoft.AspNetCore.Http.Endpoint;

namespace EndpointTranslator.Endpoints.DataSources;

public class StaticEndpointDataSource : MutableEndpointDataSource
{
    private readonly ITranslatorStrategy _translatorStrategy;
    private readonly IHttpContextRequestJsonConverter _jsonConverter;
    public StaticEndpointDataSource(
        IOptionsMonitor<AppSettings> options, 
        ITranslatorStrategy translatorStrategy, 
        IHttpContextRequestJsonConverter jsonConverter)
    {
        _translatorStrategy = translatorStrategy;
        _jsonConverter = jsonConverter;
        SetEndpoints(MakeEndpoints(options.CurrentValue));
        options.OnChange(config => SetEndpoints(MakeEndpoints(config)));
    }

    private IReadOnlyList<Endpoint> MakeEndpoints(AppSettings config) =>
        config
            .Translations
            .Select(route => CreatePostEndpoint(
                route.Inbound.Url,
                async context =>
                {
                    var json = await _jsonConverter.GetRequestBodyAsJsonAsync(context, context.RequestAborted);
                    var accumResult = GetAccumulatedResultAsync(json, route, context.RequestAborted);
                    
                    await context.Response.WriteAsync(accumResult.ToString()!);
                }))
            .ToList();

    private async Task<object> GetAccumulatedResultAsync(
        string source,
        TranslationSettings translationSettings,
        CancellationToken cancellationToken)
    {
        object accumResult = source;
        foreach (var t in translationSettings.Translators)
        {
            var translator = _translatorStrategy.CreateTranslator(t.Name);
            var parameters = CreateTranslatorParameters(
                translator.GetParametersType(), 
                t.Parameters);
            var typeInterfaces = translator.GetType()
                .GetInterface(typeof(ITranslator<,,>).Name.ToString())!
                .GenericTypeArguments;
            var input = accumResult.CastToReflected(typeInterfaces[0]);
            var castedParam = parameters?.CastToReflected(typeInterfaces[1]);

            accumResult = await ((dynamic) translator)
                .TranslateAsync(input, castedParam, cancellationToken);
        }

        return accumResult;
    }
    
    private static ITranslatorParameters? CreateTranslatorParameters(
        Type parameterType, 
        object parameters)
    {
        if (Activator.CreateInstance(parameterType) is not ITranslatorParameters instance) 
            return null;
        
        instance.LoadFromObject(parameters);

        return instance;
    }

    private static ScriptObject BuildScriptObject(ExpandoObject expando)
    {
        var dict = ((IDictionary<string, object>) expando!)!;
        var scriptObject = new ScriptObject();

        foreach (var kv in dict)
        {
            var renamedKey = StandardMemberRenamer.Rename(kv.Key);
            var value = kv.Value is ExpandoObject expandoValue
                ? BuildScriptObject(expandoValue)
                : kv.Value;
            scriptObject.Add(renamedKey, value);
        }

        return scriptObject;
    }

    private static Endpoint CreatePostEndpoint(string pattern, RequestDelegate requestDelegate) =>
        new RouteEndpointBuilder(
                requestDelegate: requestDelegate,
                routePattern: RoutePatternFactory.Parse(pattern),
                order: 0)
            {
                Metadata =
                {
                    new HttpMethodMetadata(new[] {HttpMethods.Post})
                }
            }
            .Build();

    private static Endpoint CreateEndpoint(
        string pattern,
        RequestDelegate requestDelegate) =>
        new RouteEndpointBuilder(
                requestDelegate: requestDelegate,
                routePattern: RoutePatternFactory.Parse(pattern),
                order: 0)
            .Build();
}
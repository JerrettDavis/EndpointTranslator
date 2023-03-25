using System.Dynamic;
using EndpointTranslator.Abstractions.Translators;
using Newtonsoft.Json;
using Scriban;
using Scriban.Runtime;

namespace EndpointTranslator.Plugins.Translators.JsonTemplateTranslator;

public class JsonTemplateTranslator : 
    ITranslator<string, JsonTemplateTranslatorParameters, string>
{
    public async Task<string> TranslateAsync(
        string source,
        JsonTemplateTranslatorParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var expando = JsonConvert.DeserializeObject<ExpandoObject>(source)!;
        var sObject = BuildScriptObject(expando);
        var templateContext = new TemplateContext();

        templateContext.PushGlobal(sObject);

        var template = Template.Parse(parameters.Template);

        return await template.RenderAsync(templateContext);
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

    public Type GetParametersType() => typeof(JsonTemplateTranslatorParameters);
}
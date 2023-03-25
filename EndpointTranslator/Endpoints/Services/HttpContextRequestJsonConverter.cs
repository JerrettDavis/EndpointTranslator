using System.Net.Mime;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace EndpointTranslator.Endpoints.Services;

/// <summary>
/// Attempts to convert incoming request data to a valid json string
/// </summary>
public class HttpContextRequestJsonConverter : IHttpContextRequestJsonConverter
{
    public async Task<string> GetRequestBodyAsJsonAsync(
        HttpContext context, 
        CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync(cancellationToken);
        string json;
        
        if (context.Request.ContentType == MediaTypeNames.Application.Xml ||
            body.StartsWith("<?xml")) // xml 
        {
            var doc = XDocument.Parse(body); //or XDocument.Load(path)
            json = JsonConvert.SerializeXNode(doc.Root, Formatting.None, true);
        }
        else // json
        {
            json = body;
        }

        return json;
    }
}
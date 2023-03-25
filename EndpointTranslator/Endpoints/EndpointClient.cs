using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Endpoint = EndpointTranslator.Models.Endpoint;
using HttpHeaders = EndpointTranslator.Common.Constants.HttpHeaders;

namespace EndpointTranslator.Endpoints;

public class EndpointClient
{
    private readonly HttpClient _httpClient;

    public EndpointClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> SendAsync(
        Endpoint endpoint, 
        string body, 
        CancellationToken cancellationToken)
    {
        var message = new HttpRequestMessage
        {
            Method = endpoint.Method,
            RequestUri = new Uri(endpoint.Url),
            Content = new StringContent(body, Encoding.UTF8)
        };
        message.Headers.Add(HttpHeaders.ContentType, MediaTypeNames.Application.Json);
        
        return _httpClient.SendAsync(message, cancellationToken);
    }
}
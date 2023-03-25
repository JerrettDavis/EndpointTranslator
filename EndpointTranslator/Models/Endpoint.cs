namespace EndpointTranslator.Models;

public class Endpoint
{
    public string Name { get; set; } = null!;
    public string Url { get; set; } = null!;
    public HttpMethod Method { get; set; } = null!;
}
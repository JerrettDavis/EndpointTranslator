namespace EndpointTranslator.Endpoints.Services;

public interface IHttpContextRequestJsonConverter
{
    Task<string> GetRequestBodyAsJsonAsync(HttpContext context, CancellationToken cancellationToken = default);
}
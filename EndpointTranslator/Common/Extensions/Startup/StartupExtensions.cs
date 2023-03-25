using EndpointTranslator.Endpoints.DataSources;

namespace EndpointTranslator.Common.Extensions.Startup;

public static class StartupExtensions
{
    public static void UseIntegratorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var dataSource = endpoints.ServiceProvider.GetService<StaticEndpointDataSource>();

        if (dataSource is null)
            throw new Exception("Did you forget to call Services.AddIntegratorEndpoints()?");

        endpoints.DataSources.Add(dataSource);
    }
}
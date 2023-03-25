using System.IO.Abstractions;
using EndpointTranslator.Abstractions.Translators;
using EndpointTranslator.Common.Extensions.Startup;
using EndpointTranslator.Endpoints;
using EndpointTranslator.Endpoints.DataSources;
using EndpointTranslator.Endpoints.Services;
using EndpointTranslator.Models;
using EndpointTranslator.Plugins;
using EndpointTranslator.Translators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddHttpClient<EndpointClient>();
builder.Services
    .AddScoped<IFileSystem, FileSystem>()
    .AddSingleton<StaticEndpointDataSource>()
    .AddTransient<ITranslatorStrategy, TranslatorStrategy>()
    .AddTransient<IHttpContextRequestJsonConverter, HttpContextRequestJsonConverter>()
    .AddTransient<IPluginFileProvider, PluginFileProvider>();

builder.Services
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

var plugins = await builder.LoadPluginsAsync().ToListAsync();

foreach (var plugin in plugins)
    await plugin.StartupAsync(builder.Host, builder.Services, builder.Configuration);

var app = builder.Build();

foreach (var plugin in plugins)
    await plugin.ConfigureAsync(app);

app.UseIntegratorEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();
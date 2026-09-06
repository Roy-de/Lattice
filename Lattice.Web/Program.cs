using Lattice.Shared.DesignSystem.Typography;
using Lattice.Web.Components;
using Lattice.Shared.Services;
using Lattice.Web.Services;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Register HttpClient for web resource loading
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["BaseUrl"] ?? "https://localhost:5001/") });
// Or use IHttpClientFactory
builder.Services.AddHttpClient();

// Add device-specific services used by the Lattice.Shared project
builder.Services.AddSingleton<IResourceLoader, WebResourceLoader>();
builder.Services.AddSingleton<TypographyInitializer>();
builder.Services.AddSingleton<ITypographyCssGenerator, TypographyCssGenerator>();
builder.Services.AddSingleton<ITypographyService, TypographyService>();
// builder.Services.AddSingleton<IApplicationIcons, WebApplicationIcons>(); // You'll need to create this

// Add logging
builder.Services.AddLogging(configure => 
{
    configure.AddConsole();
    configure.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(Lattice.Shared._Imports).Assembly,
        typeof(Lattice.Web.Client._Imports).Assembly);

// Initialize services asynchronously
_ = Task.Run(async () => await InitializeServicesAsync(app.Services));

app.Run();

static async Task InitializeServicesAsync(IServiceProvider services)
{
    try 
    {
        var logger = services.GetService<ILogger<Program>>();
        
        // Log all available embedded resources
        var assembly = typeof(Program).Assembly;
        var resources = assembly.GetManifestResourceNames();
        logger?.LogInformation("Total embedded resources: {Count}", resources.Length);
        
        // Log font-related resources
        var fontResources = resources.Where(r => 
            r.Contains("Font", StringComparison.OrdinalIgnoreCase) || 
            r.EndsWith(".ttf") || 
            r.EndsWith(".json"));
        
        foreach (var res in fontResources)
        {
            logger?.LogInformation("Found font resource: {Resource}", res);
        }
        
        var typography = services.GetRequiredService<TypographyInitializer>();
        await typography.InitializeAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetService<ILogger<Program>>();
        logger?.LogError(ex, "Failed to initialize services");
    }
}
using Microsoft.Extensions.Logging;
using Lattice.Shared.Services;
using Lattice.Services;
using Lattice.Services.IconService;
using Lattice.Shared.Resource;
using Lattice.Shared.Typography;

namespace Lattice;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // Add device-specific services used by the Lattice.Shared project
        builder.Services.AddSingleton<IResourceLoader, MauiResourceLoader>();
        builder.Services.AddSingleton<TypographyInitializer>();
        builder.Services.AddSingleton<ITypographyCssGenerator, TypographyCssGenerator>();
        builder.Services.AddSingleton<ITypographyService, TypographyService>();
        builder.Services.AddSingleton<IApplicationIcons, ApplicationIcons>();
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddMauiBlazorWebView();
        

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        /*var typography = app.Services.GetRequiredService<TypographyInitializer>();
        app.Services.GetRequiredService<IApplicationIcons>();*/
        _ = Task.Run(async () => await InitializeServicesAsync(app.Services));
        return app;
    }
    
    // In MauiProgram.cs - Add more logging
    private static async Task InitializeServicesAsync(IServiceProvider services)
    {
        try 
        {
            var logger = services.GetService<ILogger<App>>();
        
            // Log all available embedded resources
            var assembly = typeof(App).Assembly;
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
            var logger = services.GetService<ILogger<App>>();
            logger?.LogError(ex, "Failed to initialize services");
        }
    }
}
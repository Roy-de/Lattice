using Microsoft.Extensions.Logging;
using Lattice.Shared.Services;
using Lattice.Services;
using Lattice.Services.IconService;
using Lattice.Shared.DesignSystem.Colors;
using Lattice.Shared.DesignSystem.Motion;
using Lattice.Shared.DesignSystem.Typography;
using Lattice.Shared.Resource;

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
        builder.Services.AddSingleton<ColorPaletteInitializer>();
        builder.Services.AddSingleton<IColorPaletteService, ColorPaletteService>();
        builder.Services.AddSingleton<IColorPaletteCssGenerator, ColorPaletteCssGenerator>();
        builder.Services.AddSingleton<MotionInitializer>();
        builder.Services.AddSingleton<IMotionService, MotionService>();
        builder.Services.AddSingleton<IMotionCssGenerator, MotionCssGenerator>();
        builder.Services.AddMauiBlazorWebView();
        

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
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
            var allResources = resources.Where(r => 
                r.EndsWith(".ttf") || 
                r.EndsWith(".json"));
        
            foreach (var res in allResources)
            {
                logger?.LogInformation("Found all resource: {Resource}", res);
            }
        
            var typography = services.GetRequiredService<TypographyInitializer>();
            await typography.InitializeAsync();
            
            var colors =  services.GetRequiredService<ColorPaletteInitializer>();
            await colors.InitializeAsync();
            
            var motion = services.GetRequiredService<MotionInitializer>();
            await motion.InitializeAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetService<ILogger<App>>();
            logger?.LogError(ex, "Failed to initialize services");
        }
    }
}
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorPaletteInitializer(IColorPaletteService colorPaletteService, IResourceLoader resources, ILogger<ColorPaletteInitializer>? logger = null)
{
    public async Task InitializeAsync()
    {
        try
        {
            // List all files in the Colors directory
            var files = await resources.ListAsync("Colors/");
            var paletteFiles = files
                .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();
            
            logger?.LogInformation("Found {Count} color palette files", paletteFiles.Count);
            
            if (paletteFiles.Count == 0)
            {
                logger?.LogWarning("No color palette JSON files found in the Colors directory");
                return;
            }
            
            // Load all palettes
            foreach (var paletteId in paletteFiles)
            {
                try
                {
                    await colorPaletteService.LoadAsync("Resources/" + paletteId);
                    logger?.LogDebug("Successfully loaded color palette '{PaletteId}'", paletteId);
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, "Failed to load color palette '{PaletteId}'", paletteId);
                }
            }
            
            // Log summary after loading all palettes
            logger?.LogInformation(
                "Color palette initialization complete. Loaded {LoadedCount}/{TotalCount} palettes", 
                colorPaletteService.Palettes.Count, 
                paletteFiles.Count);
            
            // Log current palette if set
            if (colorPaletteService.Current != null)
            {
                logger?.LogInformation(
                    "Current palette: {PaletteName} (ID: {PaletteId}) with {ColorCount} colors",
                    colorPaletteService.Current.Name,
                    colorPaletteService.Current.Id,
                    colorPaletteService.Current.Colors?.Count ?? 0);
            }
            else if (colorPaletteService.Palettes.Count > 0)
            {
                // If no current palette was set, switch to first one
                var firstPalette = colorPaletteService.Palettes.First();
                colorPaletteService.SwitchToPalette(firstPalette.Id);
                logger?.LogInformation("Set first palette as current: {PaletteName}", firstPalette.Name);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to initialize color palette system");
            throw;
        }
    }
}
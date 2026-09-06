using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Typography;

public sealed class TypographyInitializer(ITypographyService typography, IResourceLoader resources, ILogger<TypographyInitializer>? logger = null)
{
    public async Task InitializeAsync()
    {
        try
        {
            // List all files in the Fonts directory
            var files = await resources.ListAsync("Fonts/");
            var presets = files
                .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();
            
            logger?.LogInformation("Found {Count} typography preset files", presets.Count);
            
            foreach (var preset in presets)
            {
                try
                {
                    await typography.LoadAsync(preset);
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, "Failed to load typography preset '{Preset}'", preset);
                }
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to initialize typography system");
            throw;
        }
    }
}
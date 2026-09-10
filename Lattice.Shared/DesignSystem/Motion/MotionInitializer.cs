using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Motion;

public sealed class MotionInitializer(IMotionService motionService, IResourceLoader resourceLoader, ILogger<MotionInitializer>? logger = null)
{
    public async Task InitializeAsync()
    {
        try
        {
            var files = await resourceLoader.ListAsync("Motion/");
            var motionFiles = files
            .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .ToList();
            
            logger?.LogInformation("Found {Count} motion files", motionFiles.Count);

            if (motionFiles.Count == 0)
            {
                logger?.LogWarning("No motion JSON files found in the motion directory");
                return;
            }

            foreach (var motionId in motionFiles)
            {
                try
                {
                    await motionService.LoadAsync(motionId);
                    logger?.LogDebug("Successfully loaded motion '{MotionId}'", motionId);
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, "Failed to load motion '{MotionId}'", motionId);
                }
                
                logger?.LogInformation("Motion initialization complete. Loaded {LoadedCount}/{TotalCount} motions",
                    motionService.MotionSystems.Count,
                    motionFiles.Count);
                
                // Log current palette if set
                if (motionService.Current != null)
                {
                    logger?.LogInformation(
                        "Current motion: {MotionName} (ID: {MotionId})",
                        motionService.Current.Name,
                        motionService.Current.Id);
                }
                else if (motionService.MotionSystems.Count > 0)
                {
                    // If no current palette was set, switch to first one
                    var motionSystem = motionService.MotionSystems.First();
                    motionService.SwitchMotionSystem(motionSystem.Id);
                    logger?.LogInformation("Set first motion as current: {PaletteName}", motionSystem.Name);
                }
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to initialize motion system");
            throw;
        }
    }
}
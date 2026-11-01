using System.Text.Json;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Colors;

public class ColorInitializer(IResourceLoader resources, ILogger<ColorInitializer>? logger = null)
{
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    public async Task initializeAsync()
    {
        try
        {
            var files = await resources.ListAsync("Colors/");
            var colorTokens = files
                .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();
            
            logger?.LogInformation("Found {Count} color token files", colorTokens.Count);

            if (colorTokens.Count == 0)
            {
                logger?.LogWarning("No color token JSON file found");
                return;
            }

            foreach (var token in colorTokens)
            {
                try
                {
                    var path = $"Colors/{token}.json";
                    await using var stream = await resources.OpenAsync("Colors", path);
                    logger?.LogDebug("Successfully loaded color token '{token}'", token);

                    using var reader = new StreamReader(stream, leaveOpen: true);

                    var json = await reader.ReadToEndAsync();

                    var loadedToken = JsonSerializer.Deserialize<ColorThemeCollection>(json, _options)
                                      ?? throw new InvalidOperationException(
                                          $"Failed to deserialize color token '{token}'.");
                    return;
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Failed to load color token '{token}'", token);
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to initialize color palette system");
            throw;
        }
    }
}
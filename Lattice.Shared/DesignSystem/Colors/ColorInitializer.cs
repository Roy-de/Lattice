using System.Text.Json;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Colors;

public class ColorInitializer(
    IResourceLoader resources,
    ILogger<ColorInitializer>? logger = null)
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task InitializeAsync()
    {
        try
        {
            var files = await resources.ListAsync("Colors/");

            var colorFiles = files
                .Where(f => f.EndsWith(
                    ".json",
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            logger?.LogInformation(
                "Found {Count} color token files",
                colorFiles.Count);

            if (colorFiles.Count == 0)
            {
                logger?.LogWarning("No color token JSON file found");
                return;
            }

            foreach (var file in colorFiles)
            {
                try
                {
                    await using var stream = await resources.OpenAsync(file);

                    var loadedToken =
                        await JsonSerializer.DeserializeAsync<ColorThemeCollection>(
                            stream,
                            _options);

                    if (loadedToken is null)
                    {
                        throw new InvalidOperationException(
                            $"Failed to deserialize color token '{file}'.");
                    }

                    var tokenName = Path.GetFileNameWithoutExtension(file);

                    logger?.LogInformation(
                        "Successfully loaded color token '{Token}'",
                        tokenName);

                    // Register/store loadedToken in your color registry here.
                }
                catch (Exception ex)
                {
                    logger?.LogError(
                        ex,
                        "Failed to load color token '{File}'",
                        file);

                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(
                ex,
                "Failed to initialize color palette system");

            throw;
        }
    }
}
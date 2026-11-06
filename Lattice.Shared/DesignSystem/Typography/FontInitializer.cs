using System.Text.Json;
using Lattice.Shared.DesignSystem.Typography.Model;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Typography;

public sealed class FontInitializer(
    IResourceLoader resources,
    ILogger<FontInitializer>? logger = null)
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly Dictionary<string, FontTheme> _themes =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, FontTheme> Themes => _themes;

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        try
        {
            var files = await resources.ListAsync("Fonts", ct);

            var fontThemes = files
                .Where(f => f.EndsWith(
                    ".json",
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            logger?.LogInformation(
                "Found {Count} typography theme files",
                fontThemes.Count);

            foreach (var file in fontThemes)
            {
                ct.ThrowIfCancellationRequested();

                await using var stream =
                    await resources.OpenAsync(file, ct);

                var theme =
                    await JsonSerializer.DeserializeAsync<FontTheme>(
                        stream,
                        _options,
                        ct);

                if (theme is null)
                    throw new InvalidOperationException(
                        $"Failed to deserialize typography theme '{file}'.");

                _themes[theme.Name] = theme;

                logger?.LogInformation(
                    "Loaded typography theme '{Theme}'",
                    theme.Name);
            }

            logger?.LogInformation(
                "Typography initialization completed. Loaded {Count} themes",
                _themes.Count);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to initialize typography system");
            throw;
        }
    }
}
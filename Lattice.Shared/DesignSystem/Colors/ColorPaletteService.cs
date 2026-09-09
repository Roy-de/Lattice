using System.Text.Json;
using Microsoft.Extensions.Logging;
using Lattice.Shared.Resource;

namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorPaletteService : IColorPaletteService
{
    private readonly IResourceLoader _resources;
    private readonly ILogger<ColorPaletteService> _logger;
    private readonly Dictionary<string, ColorPalette> _palettes = new(StringComparer.OrdinalIgnoreCase);
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
    private string? _allPalettesCss;
    private readonly Dictionary<string, string> _paletteCssCache = new(StringComparer.OrdinalIgnoreCase);

    public ColorPaletteService(IResourceLoader resources, ILogger<ColorPaletteService> logger) => (_resources, _logger) = (resources, logger);
    public ColorPalette? Current { get; private set; }
    public IReadOnlyList<ColorPalette> Palettes => _palettes.Values.ToList().AsReadOnly();

    public async Task<ColorPalette> LoadAsync(string paletteId)
    {
        var path = $"Colors/{paletteId}.json";
        try
        {
            await using var stream = await _resources.OpenAsync("Colors", path);
            using var reader = new StreamReader(stream, leaveOpen: true);
            var json = await reader.ReadToEndAsync();
            var palette = JsonSerializer.Deserialize<ColorPalette>(json, _options)
                ?? throw new InvalidOperationException($"Failed to deserialize color palette '{paletteId}'.");
            palette.Id = string.IsNullOrWhiteSpace(palette.Id) ? paletteId : palette.Id;
            if (palette.Semantic.ValueKind != JsonValueKind.Object)
                throw new InvalidOperationException($"Palette '{palette.Id}' must contain a semantic token object.");

            _palettes[palette.Id] = palette;
            Current ??= palette;
            _allPalettesCss = null;
            _paletteCssCache.Clear();
            _logger.LogInformation("Loaded palette {Name} ({Id}) with {TokenCount} semantic tokens", palette.Name, palette.Id, ColorTokenResolver.GetSemanticTokens(palette).Count);
            return palette;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load color palette '{PaletteId}'", paletteId);
            throw;
        }
    }

    public async Task<IEnumerable<ColorPalette>> LoadMultipleAsync(IEnumerable<string> paletteIds)
    {
        var palettes = new List<ColorPalette>();
        foreach (var id in paletteIds)
            try { palettes.Add(await LoadAsync(id)); }
            catch (Exception ex) { _logger.LogWarning(ex, "Skipping palette '{PaletteId}'", id); }
        return palettes;
    }

    public ColorPalette? GetPalette(string paletteId) => _palettes.GetValueOrDefault(paletteId);
    public IReadOnlyList<ColorPalette> GetPalettesSorted() => _palettes.Values.OrderBy(p => p.Name).ToList().AsReadOnly();
    public void SwitchToPalette(string paletteId) => Current = GetPalette(paletteId) ?? throw new KeyNotFoundException($"Palette '{paletteId}' has not been loaded.");
    public bool IsLoaded(string paletteId) => _palettes.ContainsKey(paletteId);

    public bool Unload(string paletteId)
    {
        if (!_palettes.Remove(paletteId)) return false;
        if (Current?.Id.Equals(paletteId, StringComparison.OrdinalIgnoreCase) == true) Current = _palettes.Values.FirstOrDefault();
        _allPalettesCss = null;
        _paletteCssCache.Remove(paletteId);
        return true;
    }

    public void ClearAll() { _palettes.Clear(); Current = null; _allPalettesCss = null; _paletteCssCache.Clear(); }
    public string GetAllPalettesCss(IColorPaletteCssGenerator generator) => _allPalettesCss ??= generator.GenerateAll(_palettes.Values);
    public string GetPaletteCss(string paletteId, IColorPaletteCssGenerator generator) => _paletteCssCache.TryGetValue(paletteId, out var css) ? css : _paletteCssCache[paletteId] = generator.Generate(GetPalette(paletteId) ?? throw new KeyNotFoundException($"Palette '{paletteId}' not found."));
}

using System.Text.Json;
using System.Text.Json.Serialization;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorPaletteService : IColorPaletteService
{
    private readonly IResourceLoader _resources;
    private readonly ILogger<ColorPaletteService> _logger;
    private readonly Dictionary<string, ColorPalette> _palettes = new();
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true)
    }
    };
    
    private string? _allPalettesCss;
    private readonly Dictionary<string, string> _paletteCssCache = new();
    
    public ColorPaletteService(IResourceLoader resources, ILogger<ColorPaletteService> logger)
    {
        _resources = resources;
        _logger = logger;
    }

    public ColorPalette? Current { get; private set; }
    public IReadOnlyList<ColorPalette> Palettes => _palettes.Values.ToList().AsReadOnly();

    public async Task<ColorPalette> LoadAsync(string paletteId)
    {
        try
        {
            var path = $"Colors/{paletteId}.json";
            _logger.LogInformation("Loading color palette from {Path}", path);
        
            await using var stream = await _resources.OpenAsync("Colors", path);
        
            using var reader = new StreamReader(stream);
            var jsonString = await reader.ReadToEndAsync();
            _logger.LogDebug("JSON content (first 200 chars): {Json}", 
                jsonString.Length > 200 ? jsonString.Substring(0, 200) + "..." : jsonString);
        
            stream.Seek(0, SeekOrigin.Begin);
        
            var palette = await JsonSerializer.DeserializeAsync<ColorPalette>(stream, _options);

            if (palette == null)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize color palette '{paletteId}'.");
            }

            if (string.IsNullOrWhiteSpace(palette.Id))
            {
                palette.Id = paletteId;
            }

            ValidatePalette(palette);

            palette.Colors = palette.Colors
                .OrderBy(c => c.Hierarchy)
                .ToList();
        
            // Ensure the palette has an ID
            if (string.IsNullOrEmpty(palette.Id))
            {
                palette.Id = paletteId;
            }
        
            // Sort colors by hierarchy if available
            if (palette.Colors != null)
            {
                palette.Colors = palette.Colors.OrderBy(c => c.Hierarchy).ToList();
            }
        
            _logger.LogInformation("Loaded palette - Name: {Name}, ID: {Id}, Colors: {Count}", 
                palette.Name, palette.Id, palette.Colors?.Count ?? 0);
        
            // Store or update the palette
            _palettes[palette.Id] = palette;
            
            // If no current palette is set, set this as current
            if (Current == null)
            {
                Current = palette;
                _logger.LogInformation("Set as current palette: {Name}", palette.Name);
            }
            
            // Clear cached CSS
            _allPalettesCss = null;
            _paletteCssCache.Clear();
        
            _logger.LogInformation("Successfully loaded palette '{PaletteId}'. Total palettes: {Total}", 
                paletteId, _palettes.Count);
                
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
        var results = new List<ColorPalette>();
        
        foreach (var paletteId in paletteIds)
        {
            try
            {
                var palette = await LoadAsync(paletteId);
                results.Add(palette);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load palette '{PaletteId}', continuing with others", paletteId);
                // Continue loading other palettes even if one fails
            }
        }
        
        _logger.LogInformation("Loaded {SuccessCount}/{TotalCount} palettes", 
            results.Count, paletteIds.Count());
        
        return results;
    }

    public ColorPalette? GetPalette(string paletteId)
    {
        return _palettes.TryGetValue(paletteId, out var palette) ? palette : null;
    }

    public IReadOnlyList<ColorPalette> GetPalettesSorted()
    {
        return _palettes.Values
            .OrderBy(p => p.Name)
            .ToList()
            .AsReadOnly();
    }

    public IEnumerable<ColorDefinition> GetAllColors()
    {
        return _palettes.Values
            .SelectMany(p => p.Colors ?? new List<ColorDefinition>())
            .DistinctBy(c => c.Id)
            .OrderBy(c => c.Name);
    }

    public IEnumerable<ColorDefinition> GetPaletteColors(string paletteId)
    {
        if (!_palettes.TryGetValue(paletteId, out var palette))
        {
            throw new KeyNotFoundException($"Palette '{paletteId}' not found");
        }
        
        return palette.Colors ?? new List<ColorDefinition>();
    }

    public void SwitchToPalette(string paletteId)
    {
        if (!_palettes.TryGetValue(paletteId, out var palette))
        {
            throw new KeyNotFoundException(
                $"Color palette '{paletteId}' has not been loaded. Call LoadAsync first.");
        }
        
        Current = palette;
        _logger.LogInformation("Switched to color palette: {Name} (ID: {Id})", 
            palette.Name, palette.Id);
    }

    public bool IsLoaded(string paletteId)
    {
        return _palettes.ContainsKey(paletteId);
    }

    public bool Unload(string paletteId)
    {
        if (!_palettes.Remove(paletteId))
            return false;
            
        // If we unloaded the current palette, try to set a new current
        if (Current?.Id == paletteId)
        {
            Current = _palettes.Values.FirstOrDefault();
            if (Current != null)
            {
                _logger.LogInformation("Set new current palette to: {Name}", Current.Name);
            }
            else
            {
                _logger.LogInformation("No palettes remaining, current palette set to null");
            }
        }
        
        _allPalettesCss = null;
        _paletteCssCache.Remove(paletteId);
        
        _logger.LogInformation("Unloaded palette: {PaletteId}", paletteId);
        return true;
    }

    public void ClearAll()
    {
        _palettes.Clear();
        Current = null;
        _allPalettesCss = null;
        _paletteCssCache.Clear();
        _logger.LogInformation("Cleared all loaded palettes");
    }

    public string GetAllPalettesCss(IColorPaletteCssGenerator generator)
    {
        if (_allPalettesCss == null)
        {
            _allPalettesCss = generator.GenerateAll(_palettes.Values.ToList());
        }
        return _allPalettesCss;
    }

    public string GetPaletteCss(string paletteId, IColorPaletteCssGenerator generator)
    {
        if (!_paletteCssCache.TryGetValue(paletteId, out var css))
        {
            if (!_palettes.TryGetValue(paletteId, out var palette))
            {
                throw new KeyNotFoundException($"Palette '{paletteId}' not found");
            }
            
            css = generator.Generate(palette);
            _paletteCssCache[paletteId] = css;
        }
        return css;
    }
    
    private static void ValidatePalette(ColorPalette palette)
    {
        var duplicateRoles = palette.Colors
            .GroupBy(c => c.Role)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateRoles.Count > 0)
        {
            throw new InvalidOperationException(
                $"Palette '{palette.Id}' contains duplicate semantic roles: " +
                string.Join(", ", duplicateRoles));
        }
    }
}
namespace Lattice.Shared.DesignSystem.Colors;

public interface IColorPaletteService
{
    // Load a palette by ID (adds to collection)
    Task<ColorPalette> LoadAsync(string paletteId);
    
    // Load multiple palettes at once
    Task<IEnumerable<ColorPalette>> LoadMultipleAsync(IEnumerable<string> paletteIds);
    
    // Get all loaded palettes
    IReadOnlyList<ColorPalette> Palettes { get; }
    
    // Get a specific palette by ID
    ColorPalette? GetPalette(string paletteId);
    
    // Get all palettes sorted by name or custom order
    IReadOnlyList<ColorPalette> GetPalettesSorted();
    
    // Switch active/current palette
    void SwitchToPalette(string paletteId);
    
    // Current active palette
    ColorPalette? Current { get; }
    
    // Check if a palette is loaded
    bool IsLoaded(string paletteId);
    
    // Unload a palette
    bool Unload(string paletteId);
    
    // Clear all loaded palettes
    void ClearAll();
    
    // Generate CSS for all palettes
    string GetAllPalettesCss(IColorPaletteCssGenerator generator);
    
    // Generate CSS for a specific palette
    string? GetPaletteCss(string paletteId);
}

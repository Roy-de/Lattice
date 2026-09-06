namespace Lattice.Shared.DesignSystem.Colors;

public interface IColorPaletteCssGenerator
{
    string Generate(ColorPalette palette);
    
    // Generate CSS for multiple palettes with class prefixes
    string GenerateAll(IEnumerable<ColorPalette> palettes);
}
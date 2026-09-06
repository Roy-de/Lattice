namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorPalette
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public List<ColorDefinition> Colors { get; set; } = [];
}
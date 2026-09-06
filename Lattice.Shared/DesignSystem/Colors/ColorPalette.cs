namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorPalette
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Personality { get; set; } = string.Empty;

    public string IntendedFor { get; set; } = string.Empty;

    public List<string> Keywords { get; set; } = [];

    public List<ColorDefinition> Colors { get; set; } = [];

    public List<ColorRoleDefinition> Roles { get; set; } = [];
}
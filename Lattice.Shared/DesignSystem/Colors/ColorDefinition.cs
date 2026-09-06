namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorDefinition
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Hierarchy { get; set; }

    public string Value { get; set; } = string.Empty;
}
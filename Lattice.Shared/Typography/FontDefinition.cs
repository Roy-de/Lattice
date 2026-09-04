namespace Lattice.Shared.Typography;

public sealed class FontDefinition
{
    public FontStyleDefinition Normal { get; set; } = new();

    public FontStyleDefinition? Italic { get; set; }
}
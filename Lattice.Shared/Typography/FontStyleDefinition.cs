namespace Lattice.Shared.Typography;

public sealed class FontStyleDefinition
{
    public VariableFontDefinition? Variable { get; set; }

    public Dictionary<string, string> Static { get; set; } = [];
}
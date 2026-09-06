namespace Lattice.Shared.DesignSystem.Typography;

public sealed class TextStyleDefinition
{
    public string Family { get; set; } = string.Empty;

    public int Weight { get; set; }

    public TypographyBreakpoint Desktop { get; set; } = new();

    public TypographyBreakpoint Mobile { get; set; } = new();
}
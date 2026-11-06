namespace Lattice.Shared.DesignSystem.Typography.Model;

public sealed class TextStyleDefinition
{
    public string Family { get; set; } = string.Empty;

    public int Weight { get; set; }

    public ResponsiveTextStyle Desktop { get; set; } = new();

    public ResponsiveTextStyle Mobile { get; set; } = new();
}

public sealed class ResponsiveTextStyle
{
    public string Size { get; set; } = string.Empty;

    public string LineHeight { get; set; } = string.Empty;

    public string LetterSpacing { get; set; } = string.Empty;
}
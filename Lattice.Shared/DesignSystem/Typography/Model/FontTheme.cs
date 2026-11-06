namespace Lattice.Shared.DesignSystem.Typography.Model;

public sealed class FontTheme
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Dictionary<string, FontFamilyDefinition> Fonts { get; set; } = [];

    public Dictionary<string, TextStyleDefinition> TextStyles { get; set; } = [];
}

public sealed class FontFamilyDefinition
{
    public FontVariantDefinition? Normal { get; set; }

    public FontVariantDefinition? Italic { get; set; }
}

public sealed class FontVariantDefinition
{
    public VariableFontDefinition? Variable { get; set; }

    public Dictionary<string, string> Static { get; set; } = [];
}

public sealed class VariableFontDefinition
{
    public string Path { get; set; } = string.Empty;

    public int[] Weight { get; set; } = [];
}


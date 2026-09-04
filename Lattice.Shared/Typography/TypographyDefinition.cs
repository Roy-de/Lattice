namespace Lattice.Shared.Typography;

public sealed class TypographyDefinition
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Dictionary<string, FontDefinition> Fonts { get; set; } = [];

    public Dictionary<string, TextStyleDefinition> TextStyles { get; set; } = [];
}
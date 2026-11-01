namespace Lattice.Shared.DesignSystem.Colors;

using System.Text.Json.Serialization;


public sealed class ColorThemeCollection
{
    [JsonPropertyName("themes")]
    public Dictionary<string, ColorThemeDefinition> Themes { get; set; } = new();
}

public sealed class ColorThemeDefinition
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("mode")]
    public string Mode { get; set; } = string.Empty;

    [JsonPropertyName("primitives")]
    public Dictionary<string, string> Primitives { get; set; } = new();
}
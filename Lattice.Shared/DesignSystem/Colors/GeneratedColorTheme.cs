namespace Lattice.Shared.DesignSystem.Colors;

public sealed class GeneratedColorTheme
{
    public string Name { get; init; } = string.Empty;

    public string Mode { get; init; } = string.Empty;

    public string Background { get; init; } = string.Empty;

    // Family -> Tone level -> HEX value
    public Dictionary<string, Dictionary<int, string>> Primitives { get; init; } = new();
}
namespace Lattice.Shared.DesignSystem.Colors;

/// <summary>A resolved semantic token from a palette JSON document.</summary>
public sealed record ColorToken(string Path, string Name, string Description, string LightValue, string DarkValue);

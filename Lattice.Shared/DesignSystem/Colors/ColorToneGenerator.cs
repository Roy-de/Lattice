namespace Lattice.Shared.DesignSystem.Colors;

public sealed class ColorToneGenerator
{
    private static readonly int[] ToneLevels =
    [
        50,
        100,
        200,
        300,
        400,
        500,
        600,
        700,
        800,
        900,
        950
    ];

    private const string LightBackground = "#FFFFFF";
    private const string DarkBackground = "#121212";

    public IReadOnlyList<int> Levels => ToneLevels;

    public GeneratedColorTheme Generate(ColorThemeDefinition theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        ValidateTheme(theme);

        var background = GetBackground(theme.Mode);

        var generatedPrimitives =
            new Dictionary<string, Dictionary<int, string>>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var (family, baseColor) in theme.Primitives)
        {
            var palette = GeneratePalette(
                baseColor,
                background);

            generatedPrimitives.Add(family, palette);
        }

        return new GeneratedColorTheme
        {
            Name = theme.Name,
            Mode = theme.Mode,
            Background = background,
            Primitives = generatedPrimitives
        };
    }

    public Dictionary<string, GeneratedColorTheme> GenerateAll(
        ColorThemeCollection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        var result = new Dictionary<string, GeneratedColorTheme>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var (themeKey, themeDefinition) in collection.Themes)
        {
            result.Add(themeKey, Generate(themeDefinition));
        }

        return result;
    }

    public Dictionary<int, string> GeneratePalette(
        string baseColor,
        string background)
    {
        var baseRgb = ParseHex(baseColor);
        var backgroundRgb = ParseHex(background);

        var palette = new Dictionary<int, string>();

        foreach (var level in ToneLevels)
        {
            var opacity = level / 1000.0;

            var tone = Blend(
                baseRgb,
                backgroundRgb,
                opacity);

            palette.Add(level, ToHex(tone));
        }

        return palette;
    }

    private static RgbColor Blend(
        RgbColor foreground,
        RgbColor background,
        double opacity)
    {
        var red = BlendChannel(
            foreground.R,
            background.R,
            opacity);

        var green = BlendChannel(
            foreground.G,
            background.G,
            opacity);

        var blue = BlendChannel(
            foreground.B,
            background.B,
            opacity);

        return new RgbColor(red, green, blue);
    }

    private static byte BlendChannel(
        byte foreground,
        byte background,
        double opacity)
    {
        var value =
            foreground * opacity +
            background * (1.0 - opacity);

        return (byte)Math.Clamp(
            Math.Round(value),
            0,
            255);
    }

    private static RgbColor ParseHex(string hex)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hex);

        var value = hex.Trim();

        if (value.StartsWith('#'))
        {
            value = value[1..];
        }

        if (value.Length != 6)
        {
            throw new FormatException(
                $"Invalid HEX color '{hex}'. Expected #RRGGBB.");
        }

        if (!int.TryParse(
                value,
                System.Globalization.NumberStyles.HexNumber,
                null,
                out var color))
        {
            throw new FormatException(
                $"Invalid HEX color '{hex}'.");
        }

        var red = (byte)((color >> 16) & 0xFF);
        var green = (byte)((color >> 8) & 0xFF);
        var blue = (byte)(color & 0xFF);

        return new RgbColor(red, green, blue);
    }

    private static string ToHex(RgbColor color)
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    private static string GetBackground(string mode)
    {
        return mode.Trim().ToLowerInvariant() switch
        {
            "light" => LightBackground,
            "dark" => DarkBackground,

            _ => throw new InvalidOperationException(
                $"Unsupported theme mode '{mode}'. " +
                "Expected 'light' or 'dark'.")
        };
    }

    private static void ValidateTheme(ColorThemeDefinition theme)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(theme.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(theme.Mode);

        if (theme.Primitives.Count == 0)
        {
            throw new InvalidOperationException(
                $"Theme '{theme.Name}' has no primitive colors.");
        }

        foreach (var (family, value) in theme.Primitives)
        {
            if (string.IsNullOrWhiteSpace(family))
            {
                throw new InvalidOperationException(
                    "A primitive color family has an empty name.");
            }

            // Also validates the HEX value.
            ParseHex(value);
        }
    }

    private readonly record struct RgbColor(
        byte R,
        byte G,
        byte B);
}
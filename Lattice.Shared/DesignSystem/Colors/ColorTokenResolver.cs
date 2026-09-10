using System.Text;
using System.Text.Json;

namespace Lattice.Shared.DesignSystem.Colors;

public static class ColorTokenResolver
{
    public static IReadOnlyList<ColorToken> GetSemanticTokens(ColorPalette palette)
    {
        var tokens = new List<ColorToken>();
        Visit(palette, palette.Semantic, string.Empty, tokens);
        return tokens;
    }

    /// <summary>Returns literal primitive colours, including nested scales.</summary>
    public static IReadOnlyList<ColorToken> GetPrimitiveTokens(ColorPalette palette)
    {
        var tokens = new List<ColorToken>();
        VisitPrimitives(palette.Primitives, string.Empty, tokens);
        return tokens;
    }

    public static string Resolve(ColorPalette palette, string reference, string theme) => ResolveReference(palette, reference, theme, 0);

    private static string ResolveReference(ColorPalette palette, string reference, string theme, int depth)
    {
        if (depth > 12) return string.Empty;
        if (string.IsNullOrWhiteSpace(reference)) return string.Empty;
        reference = reference.Trim().Trim('{', '}');
        if (IsCssValue(reference)) return reference;
        if (reference.StartsWith("primitives.", StringComparison.OrdinalIgnoreCase)) reference = reference[11..];
        if (TryGetPath(palette.Primitives, reference, out var primitive)) return ResolveElement(palette, primitive, theme, depth + 1);
        if (reference.StartsWith("semantic.", StringComparison.OrdinalIgnoreCase)) reference = reference[9..];
        return TryGetPath(palette.Semantic, reference, out var semantic)
            ? ResolveElement(palette, semantic, theme, depth + 1)
            : string.Empty;
    }

    public static string ToCssVariable(string prefix, string path)
    {
        return $"--{prefix}-{ToKebabCase(path)}";
    }

    private static string ToKebabCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var builder = new StringBuilder();

        for (var i = 0; i < value.Length; i++)
        {
            var character = value[i];

            if (character == '.')
            {
                if (builder.Length > 0 && builder[^1] != '-')
                    builder.Append('-');

                continue;
            }

            if (char.IsUpper(character))
            {
                if (builder.Length > 0 && builder[^1] != '-')
                    builder.Append('-');

                builder.Append(
                    char.ToLowerInvariant(character));

                continue;
            }

            builder.Append(
                char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }
    private static void Visit(ColorPalette palette, JsonElement node, string path, ICollection<ColorToken> tokens)
    {
        if (node.ValueKind != JsonValueKind.Object) return;
        if (HasValue(node))
        {
            var name = GetString(node, "name");
            tokens.Add(new ColorToken(path, string.IsNullOrWhiteSpace(name) ? path : name, GetString(node, "description"), ResolveElement(palette, node, "light", 0), ResolveElement(palette, node, "dark", 0)));
            return;
        }
        foreach (var property in node.EnumerateObject())
            Visit(palette, property.Value, string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}", tokens);
    }

    private static void VisitPrimitives(JsonElement node, string path, ICollection<ColorToken> tokens)
    {
        if (node.ValueKind == JsonValueKind.String)
        {
            var value = node.GetString() ?? string.Empty;
            if (IsCssValue(value)) tokens.Add(new ColorToken(path, path, string.Empty, value, value));
            return;
        }
        if (node.ValueKind != JsonValueKind.Object) return;
        foreach (var property in node.EnumerateObject())
        {
            // Primitive metadata is useful to authors but is not a colour token.
            if (property.Name is "id" or "name" or "description") continue;
            VisitPrimitives(property.Value, string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}", tokens);
        }
    }

    private static string ResolveElement(ColorPalette palette, JsonElement element, string theme, int depth)
    {
        if (depth > 12) return string.Empty;
        if (element.ValueKind == JsonValueKind.String) return ResolveReference(palette, element.GetString() ?? string.Empty, theme, depth + 1);
        if (element.ValueKind != JsonValueKind.Object) return string.Empty;
        if (TryGetProperty(element, theme, out var themed)) return ResolveElement(palette, themed, theme, depth + 1);
        if (TryGetProperty(element, "value", out var value)) return ResolveElement(palette, value, theme, depth + 1);
        return string.Empty;
    }

    private static bool HasValue(JsonElement element) => TryGetProperty(element, "light", out _) || TryGetProperty(element, "dark", out _) || TryGetProperty(element, "value", out _);
    private static string GetString(JsonElement element, string name) => TryGetProperty(element, name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() ?? string.Empty : string.Empty;
    private static bool IsCssValue(string value) => value.StartsWith("#", StringComparison.Ordinal) || value.StartsWith("rgb", StringComparison.OrdinalIgnoreCase) || value.StartsWith("hsl", StringComparison.OrdinalIgnoreCase) || value.StartsWith("var(", StringComparison.OrdinalIgnoreCase) || value.Equals("transparent", StringComparison.OrdinalIgnoreCase) || value.Equals("currentColor", StringComparison.OrdinalIgnoreCase);

    private static bool TryGetPath(JsonElement root, string path, out JsonElement value)
    {
        value = root;
        if (root.ValueKind != JsonValueKind.Object) return false;
        foreach (var part in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            if (TryGetProperty(value, part, out var next)) { value = next; continue; }
            if (TryGetProperty(value, "scale", out var scale) && TryGetProperty(scale, part, out next)) { value = next; continue; }
            return false;
        }
        return true;
    }

    private static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
            foreach (var property in element.EnumerateObject())
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) { value = property.Value; return true; }
        value = default;
        return false;
    }
}

namespace Lattice.Shared.DesignSystem.Semantics;

public sealed class TokenResolver(TokenRegistry registry)
{
    /// <summary>
    /// Resolve a token id for a theme, honouring the document's
    /// resolution order: user-overrides → theme-mappings → token-defaults.
    /// </summary>
    public string Resolve(
        string documentId,
        string tokenId,
        string themeName,
        IReadOnlyDictionary<string, string>? userOverrides = null)
    {
        var doc = registry.RequireDocument(documentId);

        var token = doc.Tokens.FirstOrDefault(t => t.Id == tokenId)
            ?? throw new InvalidOperationException(
                $"Token '{tokenId}' not found in document '{documentId}'.");

        if (!doc.Themes.TryGetValue(themeName, out var theme))
            throw new InvalidOperationException(
                $"Theme '{themeName}' is not declared in document '{documentId}'.");

        // 1. user overrides
        if (userOverrides is not null
            && userOverrides.TryGetValue(tokenId, out var userValue))
        {
            if (!token.Customization.UserOverridable)
                throw new InvalidOperationException(
                    $"Token '{tokenId}' does not permit user overrides (policy: {token.Customization.Policy}).");

            return userValue;
        }

        // 2. theme mapping
        if (theme.Mappings.TryGetValue(tokenId, out var themeValue))
            return themeValue;

        // 3. token default (shaped differently per token type)
        return ExtractDefault(token, themeName);
    }

    private static string ExtractDefault(Token token, string themeName)
    {
        switch (token.Type)
        {
            case "color":
                if (token.Default.TryGetValue(themeName, out var themed))
                    return themed.GetString() ?? Fail(token, themeName);
                break;

            case "shadow":
            case "dimension":
            case "duration":
            case "easing":
                if (token.Default.TryGetValue("value", out var value))
                    return value.GetString() ?? Fail(token, themeName);
                // shadow tokens are still themed in the JSON
                if (token.Default.TryGetValue(themeName, out var themedShadow))
                    return themedShadow.GetString() ?? Fail(token, themeName);
                break;

            case "typography-style":
                if (token.Default.TryGetValue("reference", out var reference))
                    return reference.GetString() ?? Fail(token, themeName);
                break;
        }

        throw new InvalidOperationException(
            $"Token '{token.Id}' ({token.Type}) has no resolvable default for theme '{themeName}'.");
    }

    private static string Fail(Token token, string themeName) =>
        $"Token '{token.Id}' default value for theme '{themeName}' is null.";
}
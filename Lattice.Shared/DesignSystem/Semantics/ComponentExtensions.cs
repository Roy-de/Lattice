using System.Text.RegularExpressions;

namespace Lattice.Shared.DesignSystem.Semantics;

public static class ComponentExtensions
{
    public static string ExpandBinding(
        this Component component,
        string bindingKey,
        IReadOnlyDictionary<string, string> placeholders)
    {
        var template = component.TokenBindings[bindingKey];

        return Regex.Replace(template, @"\{(\w+)\}", m =>
        {
            var name = m.Groups[1].Value;
            return placeholders.TryGetValue(name, out var v)
                ? v
                : throw new InvalidOperationException(
                    $"Component '{component.Id}' binding '{bindingKey}' " +
                    $"references unknown placeholder '{name}'.");
        });
    }
}
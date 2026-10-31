using System.Text;
using System.Text.RegularExpressions;
using Lattice.Shared.DesignSystem.Motion.Models;

namespace Lattice.Shared.DesignSystem.Motion;

public sealed class MotionCssGenerator : IMotionCssGenerator
{
    private static readonly Regex TokenReferenceRegex =
        new(@"\{([^}]+)\}", RegexOptions.Compiled);

    public string Generate(MotionSystem motion)
    {
        ArgumentNullException.ThrowIfNull(motion);

        var css = new StringBuilder();

        AppendHeader(css, motion);

        AppendPrimitiveTokens(css, motion);

        AppendSemanticTokens(css, motion);

        AppendKeyframes(css, motion);

        AppendUtilityClasses(css, motion);

        AppendReducedMotion(css, motion);

        return css.ToString();
    }

    public string GenerateAll(IEnumerable<MotionSystem> motions)
    {
        ArgumentNullException.ThrowIfNull(motions);

        var systems = motions
            .Where(m => m is not null)
            .ToList();

        if (systems.Count == 0)
            return string.Empty;

        var css = new StringBuilder();

        css.AppendLine("/* Lattice Motion Systems */");
        css.AppendLine();

        foreach (var motion in systems)
        {
            css.AppendLine(Generate(motion));
            css.AppendLine();
        }

        return css.ToString().Trim();
    }

    private static void AppendHeader(
        StringBuilder css,
        MotionSystem motion)
    {
        css.AppendLine($"/* Motion System: {motion.Name} ({motion.Id}) */");

        if (!string.IsNullOrWhiteSpace(motion.Description))
        {
            css.AppendLine($"/* {motion.Description} */");
        }

        css.AppendLine();
    }

    private static void AppendPrimitiveTokens(
        StringBuilder css,
        MotionSystem motion)
    {
        css.AppendLine(
            $":root[data-motion=\"{motion.Id}\"] {{");

        // Durations
        AppendToken(css, "duration-instant", motion.Durations.Instant);
        AppendToken(css, "duration-micro", motion.Durations.Micro);
        AppendToken(css, "duration-fast", motion.Durations.Fast);
        AppendToken(css, "duration-normal", motion.Durations.Normal);
        AppendToken(css, "duration-moderate", motion.Durations.Moderate);
        AppendToken(css, "duration-slow", motion.Durations.Slow);
        AppendToken(css, "duration-deliberate", motion.Durations.Deliberate);

        css.AppendLine();

        // Easings
        AppendToken(css, "easing-linear", motion.Easings.Linear);
        AppendToken(css, "easing-standard", motion.Easings.Standard);
        AppendToken(css, "easing-enter", motion.Easings.Enter);
        AppendToken(css, "easing-exit", motion.Easings.Exit);

        css.AppendLine();

        // Distances
        AppendToken(css, "distance-none", motion.Distances.None);
        AppendToken(css, "distance-micro", motion.Distances.Micro);
        AppendToken(css, "distance-subtle", motion.Distances.Subtle);
        AppendToken(css, "distance-small", motion.Distances.Small);
        AppendToken(css, "distance-medium", motion.Distances.Medium);
        AppendToken(css, "distance-large", motion.Distances.Large);
        AppendToken(css, "distance-dramatic", motion.Distances.Dramatic);

        css.AppendLine();

        // Scale
        AppendToken(css, "scale-none", motion.Scale.None);
        AppendToken(css, "scale-subtle-down", motion.Scale.SubtleDown);
        AppendToken(css, "scale-small-down", motion.Scale.SmallDown);
        AppendToken(css, "scale-subtle-up", motion.Scale.SubtleUp);
        AppendToken(css, "scale-small-up", motion.Scale.SmallUp);

        css.AppendLine();

        // Opacity
        AppendToken(css, "opacity-hidden", motion.Opacity.Hidden);
        AppendToken(css, "opacity-subtle", motion.Opacity.Subtle);
        AppendToken(css, "opacity-visible", motion.Opacity.Visible);

        css.AppendLine();

        // Stagger
        AppendToken(css, "stagger-subtle", motion.Stagger.Subtle);
        AppendToken(css, "stagger-normal", motion.Stagger.Normal);
        AppendToken(css, "stagger-slow", motion.Stagger.Slow);

        css.AppendLine("}");
        css.AppendLine();
    }

    private static void AppendSemanticTokens(
        StringBuilder css,
        MotionSystem motion)
    {
        var selector = $":root[data-motion=\"{motion.Id}\"]";

        css.AppendLine($"{selector} {{");

        AppendTransitionTokens(
            css,
            "interaction-hover",
            motion.Semantic.Interaction.Hover,
            motion);

        AppendTransitionTokens(
            css,
            "interaction-press",
            motion.Semantic.Interaction.Press,
            motion);

        AppendTransitionTokens(
            css,
            "interaction-focus",
            motion.Semantic.Interaction.Focus,
            motion);

        AppendTransitionTokens(
            css,
            "surface-popover-enter",
            motion.Semantic.Surface.PopoverEnter,
            motion);

        AppendTransitionTokens(
            css,
            "surface-popover-exit",
            motion.Semantic.Surface.PopoverExit,
            motion);

        AppendTransitionTokens(
            css,
            "surface-dialog-enter",
            motion.Semantic.Surface.DialogEnter,
            motion);

        AppendTransitionTokens(
            css,
            "surface-dialog-exit",
            motion.Semantic.Surface.DialogExit,
            motion);

        AppendTransitionTokens(
            css,
            "content-fade-in",
            motion.Semantic.Content.FadeIn,
            motion);

        AppendTransitionTokens(
            css,
            "content-fade-up",
            motion.Semantic.Content.FadeUp,
            motion);

        AppendTransitionTokens(
            css,
            "content-scale-in",
            motion.Semantic.Content.ScaleIn,
            motion);

        AppendTransitionTokens(
            css,
            "feedback-notification-enter",
            motion.Semantic.Feedback.NotificationEnter,
            motion);

        AppendTransitionTokens(
            css,
            "feedback-notification-exit",
            motion.Semantic.Feedback.NotificationExit,
            motion);

        css.AppendLine("}");
        css.AppendLine();
    }

    private static void AppendTransitionTokens(
        StringBuilder css,
        string name,
        MotionTransition transition,
        MotionSystem motion)
    {
        if (transition is null)
            return;

        var duration = ResolveToken(
            transition.Duration,
            motion);

        var easing = ResolveToken(
            transition.Easing,
            motion);

        if (!string.IsNullOrWhiteSpace(duration))
        {
            AppendToken(
                css,
                $"{name}-duration",
                duration);
        }

        if (!string.IsNullOrWhiteSpace(easing))
        {
            AppendToken(
                css,
                $"{name}-easing",
                easing);
        }

        if (!string.IsNullOrWhiteSpace(transition.Distance))
        {
            AppendToken(
                css,
                $"{name}-distance",
                ResolveToken(
                    transition.Distance,
                    motion));
        }

        if (!string.IsNullOrWhiteSpace(transition.Scale))
        {
            AppendToken(
                css,
                $"{name}-scale",
                ResolveToken(
                    transition.Scale,
                    motion));
        }

        if (!string.IsNullOrWhiteSpace(transition.Transform))
        {
            AppendToken(
                css,
                $"{name}-transform",
                ResolveToken(
                    transition.Transform,
                    motion));
        }

        if (transition.Opacity is not null)
        {
            AppendToken(
                css,
                $"{name}-opacity-from",
                transition.Opacity.From);

            AppendToken(
                css,
                $"{name}-opacity-to",
                transition.Opacity.To);
        }
    }

    private static void AppendKeyframes(
        StringBuilder css,
        MotionSystem motion)
    {
        css.AppendLine(
            $"@keyframes lattice-{motion.Id}-fade-in {{");

        css.AppendLine("    from {");
        css.AppendLine("        opacity: 0;");
        css.AppendLine("    }");

        css.AppendLine("    to {");
        css.AppendLine("        opacity: 1;");
        css.AppendLine("    }");

        css.AppendLine("}");
        css.AppendLine();

        AppendFadeUpKeyframes(css, motion);

        AppendScaleInKeyframes(css, motion);

        AppendDialogKeyframes(css, motion);
    }

    private static void AppendFadeUpKeyframes(
        StringBuilder css,
        MotionSystem motion)
    {
        var distance = ResolveToken(
            motion.Semantic.Content.FadeUp.Distance,
            motion);

        if (string.IsNullOrWhiteSpace(distance))
            distance = motion.Distances.Medium;

        css.AppendLine(
            $"@keyframes lattice-{motion.Id}-fade-up {{");

        css.AppendLine("    from {");
        css.AppendLine("        opacity: 0;");
        css.AppendLine($"        transform: translateY({distance});");
        css.AppendLine("    }");

        css.AppendLine("    to {");
        css.AppendLine("        opacity: 1;");
        css.AppendLine("        transform: translateY(0);");
        css.AppendLine("    }");

        css.AppendLine("}");
        css.AppendLine();
    }

    private static void AppendScaleInKeyframes(
        StringBuilder css,
        MotionSystem motion)
    {
        var scale = ResolveToken(
            motion.Semantic.Content.ScaleIn.Scale,
            motion);

        if (string.IsNullOrWhiteSpace(scale))
            scale = motion.Scale.SubtleDown.ToString(
                System.Globalization.CultureInfo.InvariantCulture);

        css.AppendLine(
            $"@keyframes lattice-{motion.Id}-scale-in {{");

        css.AppendLine("    from {");
        css.AppendLine("        opacity: 0;");
        css.AppendLine($"        transform: scale({scale});");
        css.AppendLine("    }");

        css.AppendLine("    to {");
        css.AppendLine("        opacity: 1;");
        css.AppendLine("        transform: scale(1);");
        css.AppendLine("    }");

        css.AppendLine("}");
        css.AppendLine();
    }

    private static void AppendDialogKeyframes(
        StringBuilder css,
        MotionSystem motion)
    {
        var enter = motion.Semantic.Surface.DialogEnter;

        var distance = ResolveToken(
            enter.Distance,
            motion);

        if (string.IsNullOrWhiteSpace(distance))
            distance = motion.Distances.Medium;

        css.AppendLine(
            $"@keyframes lattice-{motion.Id}-dialog-enter {{");

        css.AppendLine("    from {");
        css.AppendLine("        opacity: 0;");
        css.AppendLine($"        transform: translateY({distance});");
        css.AppendLine("    }");

        css.AppendLine("    to {");
        css.AppendLine("        opacity: 1;");
        css.AppendLine("        transform: translateY(0);");
        css.AppendLine("    }");

        css.AppendLine("}");
        css.AppendLine();

        css.AppendLine(
            $"@keyframes lattice-{motion.Id}-dialog-exit {{");

        css.AppendLine("    from {");
        css.AppendLine("        opacity: 1;");
        css.AppendLine("        transform: translateY(0);");
        css.AppendLine("    }");

        css.AppendLine("    to {");
        css.AppendLine("        opacity: 0;");
        css.AppendLine($"        transform: translateY({distance});");
        css.AppendLine("    }");

        css.AppendLine("}");
        css.AppendLine();
    }

    private static void AppendUtilityClasses(
        StringBuilder css,
        MotionSystem motion)
    {
        var prefix = $"lattice-motion-{motion.Id}";

        var fadeIn = motion.Semantic.Content.FadeIn;

        css.AppendLine($".{prefix}-fade-in {{");
        css.AppendLine(
            $"    animation: lattice-{motion.Id}-fade-in " +
            $"{ResolveToken(fadeIn.Duration, motion)} " +
            $"{ResolveToken(fadeIn.Easing, motion)} both;");
        css.AppendLine("}");
        css.AppendLine();

        var fadeUp = motion.Semantic.Content.FadeUp;

        css.AppendLine($".{prefix}-fade-up {{");
        css.AppendLine(
            $"    animation: lattice-{motion.Id}-fade-up " +
            $"{ResolveToken(fadeUp.Duration, motion)} " +
            $"{ResolveToken(fadeUp.Easing, motion)} both;");
        css.AppendLine("}");
        css.AppendLine();

        var scaleIn = motion.Semantic.Content.ScaleIn;

        css.AppendLine($".{prefix}-scale-in {{");
        css.AppendLine(
            $"    animation: lattice-{motion.Id}-scale-in " +
            $"{ResolveToken(scaleIn.Duration, motion)} " +
            $"{ResolveToken(scaleIn.Easing, motion)} both;");
        css.AppendLine("}");
        css.AppendLine();

        var dialogEnter = motion.Semantic.Surface.DialogEnter;

        css.AppendLine($".{prefix}-dialog-enter {{");
        css.AppendLine(
            $"    animation: lattice-{motion.Id}-dialog-enter " +
            $"{ResolveToken(dialogEnter.Duration, motion)} " +
            $"{ResolveToken(dialogEnter.Easing, motion)} both;");
        css.AppendLine("}");
        css.AppendLine();

        var dialogExit = motion.Semantic.Surface.DialogExit;

        css.AppendLine($".{prefix}-dialog-exit {{");
        css.AppendLine(
            $"    animation: lattice-{motion.Id}-dialog-exit " +
            $"{ResolveToken(dialogExit.Duration, motion)} " +
            $"{ResolveToken(dialogExit.Easing, motion)} both;");
        css.AppendLine("}");
        css.AppendLine();
    }

    private static void AppendReducedMotion(
        StringBuilder css,
        MotionSystem motion)
    {
        var reduced = motion.Accessibility.ReducedMotion;

        if (!reduced.Enabled)
            return;

        css.AppendLine("@media (prefers-reduced-motion: reduce) {");

        css.AppendLine(
            $"    :root[data-motion=\"{motion.Id}\"] {{");

        css.AppendLine(
            $"        --motion-reduced-duration: {reduced.MaxDuration};");

        css.AppendLine("    }");

        css.AppendLine(
            $"    :root[data-motion=\"{motion.Id}\"] " +
            $"[data-motion-animate] {{");

        css.AppendLine(
            "        animation-duration: var(--motion-reduced-duration);");

        if (!reduced.AllowTransform)
        {
            css.AppendLine("        transform: none !important;");
        }

        if (!reduced.AllowScale)
        {
            css.AppendLine("        scale: none !important;");
        }

        css.AppendLine("    }");

        css.AppendLine("}");
        css.AppendLine();
    }

    private static string ResolveToken(
        string? value,
        MotionSystem motion)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return TokenReferenceRegex.Replace(
            value,
            match => ResolveReference(
                match.Groups[1].Value,
                motion));
    }

    private static string ResolveReference(
        string reference,
        MotionSystem motion)
    {
        var parts = reference.Split(
            '.',
            StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
            return reference;

        var category = parts[0];
        var token = parts[1];

        return category.ToLowerInvariant() switch
        {
            "durations" => ResolveDuration(token, motion),
            "easings" => ResolveEasing(token, motion),
            "distances" => ResolveDistance(token, motion),
            "scale" => ResolveScale(token, motion),
            "opacity" => ResolveOpacity(token, motion),
            "stagger" => ResolveStagger(token, motion),
            _ => reference
        };
    }

    private static string ResolveDuration(
        string token,
        MotionSystem motion)
    {
        return token.ToLowerInvariant() switch
        {
            "instant" => motion.Durations.Instant,
            "micro" => motion.Durations.Micro,
            "fast" => motion.Durations.Fast,
            "normal" => motion.Durations.Normal,
            "moderate" => motion.Durations.Moderate,
            "slow" => motion.Durations.Slow,
            "deliberate" => motion.Durations.Deliberate,
            _ => $"{{durations.{token}}}"
        };
    }

    private static string ResolveEasing(
        string token,
        MotionSystem motion)
    {
        return token.ToLowerInvariant() switch
        {
            "linear" => motion.Easings.Linear,
            "standard" => motion.Easings.Standard,
            "enter" => motion.Easings.Enter,
            "exit" => motion.Easings.Exit,
            _ => $"{{easings.{token}}}"
        };
    }

    private static string ResolveDistance(
        string token,
        MotionSystem motion)
    {
        return token.ToLowerInvariant() switch
        {
            "none" => motion.Distances.None,
            "micro" => motion.Distances.Micro,
            "subtle" => motion.Distances.Subtle,
            "small" => motion.Distances.Small,
            "medium" => motion.Distances.Medium,
            "large" => motion.Distances.Large,
            "dramatic" => motion.Distances.Dramatic,
            _ => $"{{distances.{token}}}"
        };
    }

    private static string ResolveScale(
        string token,
        MotionSystem motion)
    {
        return token.ToLowerInvariant() switch
        {
            "none" => motion.Scale.None.ToString(
                System.Globalization.CultureInfo.InvariantCulture),

            "subtledown" => motion.Scale.SubtleDown.ToString(
                System.Globalization.CultureInfo.InvariantCulture),

            "smalldown" => motion.Scale.SmallDown.ToString(
                System.Globalization.CultureInfo.InvariantCulture),

            "subtleup" => motion.Scale.SubtleUp.ToString(
                System.Globalization.CultureInfo.InvariantCulture),

            "smallup" => motion.Scale.SmallUp.ToString(
                System.Globalization.CultureInfo.InvariantCulture),

            _ => $"{{scale.{token}}}"
        };
    }

    private static string ResolveOpacity(
        string token,
        MotionSystem motion)
    {
        return token.ToLowerInvariant() switch
        {
            "hidden" => motion.Opacity.Hidden.ToString(
                System.Globalization.CultureInfo.InvariantCulture),

            "subtle" => motion.Opacity.Subtle.ToString(
                System.Globalization.CultureInfo.InvariantCulture),

            "visible" => motion.Opacity.Visible.ToString(
                System.Globalization.CultureInfo.InvariantCulture),

            _ => $"{{opacity.{token}}}"
        };
    }

    private static string ResolveStagger(
        string token,
        MotionSystem motion)
    {
        return token.ToLowerInvariant() switch
        {
            "subtle" => motion.Stagger.Subtle,
            "normal" => motion.Stagger.Normal,
            "slow" => motion.Stagger.Slow,
            _ => $"{{stagger.{token}}}"
        };
    }

    private static void AppendToken(
        StringBuilder css,
        string name,
        object? value)
    {
        if (value is null)
            return;

        var formatted = value switch
        {
            double number =>
                number.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),

            float number =>
                number.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),

            _ => value.ToString()
        };

        if (string.IsNullOrWhiteSpace(formatted))
            return;

        css.AppendLine($"    --motion-{name}: {formatted};");
    }
}

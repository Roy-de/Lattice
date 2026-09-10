namespace Lattice.Shared.DesignSystem.Motion.Models;

public sealed class MotionTransition
{
    public string Duration { get; set; } = string.Empty;

    public string Easing { get; set; } = string.Empty;

    public string? Transform { get; set; }

    public string? Scale { get; set; }

    public string? Distance { get; set; }

    public MotionOpacityRange? Opacity { get; set; }
}
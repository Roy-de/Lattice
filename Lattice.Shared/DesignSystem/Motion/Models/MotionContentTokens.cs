namespace Lattice.Shared.DesignSystem.Motion.Models;

public sealed class MotionContentTokens
{
    public MotionTransition FadeIn { get; set; } = new();

    public MotionTransition FadeUp { get; set; } = new();

    public MotionTransition ScaleIn { get; set; } = new();
}
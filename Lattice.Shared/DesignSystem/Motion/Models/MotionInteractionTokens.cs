namespace Lattice.Shared.DesignSystem.Motion.Models;

public sealed class MotionInteractionTokens
{
    public MotionTransition Hover { get; set; } = new();

    public MotionTransition Press { get; set; } = new();

    public MotionTransition Focus { get; set; } = new();
}
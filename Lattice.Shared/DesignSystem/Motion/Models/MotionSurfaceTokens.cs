namespace Lattice.Shared.DesignSystem.Motion.Models;

public sealed class MotionSurfaceTokens
{
    public MotionTransition PopoverEnter { get; set; } = new();

    public MotionTransition PopoverExit { get; set; } = new();

    public MotionTransition DialogEnter { get; set; } = new();

    public MotionTransition DialogExit { get; set; } = new();
}
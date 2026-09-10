namespace Lattice.Shared.DesignSystem.Motion.Models;

public sealed class MotionScale
{
    public double None { get; set; } = 1.0;

    public double SubtleDown { get; set; } = 0.99;

    public double SmallDown { get; set; } = 0.98;

    public double SubtleUp { get; set; } = 1.01;

    public double SmallUp { get; set; } = 1.02;
}
namespace Lattice.Shared.DesignSystem.Motion.Models;

public sealed class MotionEasings
{
    public string Linear { get; set; } = "linear";

    public string Standard { get; set; } = "cubic-bezier(0.2, 0, 0, 1)";

    public string Enter { get; set; } = "cubic-bezier(0, 0, 0.2, 1)";

    public string Exit { get; set; } = "cubic-bezier(0.4, 0, 1, 1)";
}
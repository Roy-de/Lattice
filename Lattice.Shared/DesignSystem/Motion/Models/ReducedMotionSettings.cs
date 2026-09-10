namespace Lattice.Shared.DesignSystem.Motion.Models;

public sealed class ReducedMotionSettings
{
    public bool Enabled { get; set; } = true;

    public string Strategy { get; set; } = "minimize";

    public string MaxDuration { get; set; } = "100ms";

    public bool AllowOpacity { get; set; } = true;

    public bool AllowTransform { get; set; }

    public bool AllowScale { get; set; }
}
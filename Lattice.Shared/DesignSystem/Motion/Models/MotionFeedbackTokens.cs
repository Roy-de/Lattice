namespace Lattice.Shared.DesignSystem.Motion.Models;

public sealed class MotionFeedbackTokens
{
    public MotionTransition NotificationEnter { get; set; } = new();

    public MotionTransition NotificationExit { get; set; } = new();
}
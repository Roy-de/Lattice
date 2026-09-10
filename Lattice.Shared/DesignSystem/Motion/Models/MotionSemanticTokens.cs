namespace Lattice.Shared.DesignSystem.Motion.Models;

public sealed class MotionSemanticTokens
{
    public MotionInteractionTokens Interaction { get; set; } = new();

    public MotionSurfaceTokens Surface { get; set; } = new();

    public MotionContentTokens Content { get; set; } = new();

    public MotionFeedbackTokens Feedback { get; set; } = new();
}
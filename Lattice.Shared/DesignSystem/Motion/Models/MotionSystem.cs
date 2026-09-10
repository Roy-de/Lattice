namespace Lattice.Shared.DesignSystem.Motion.Models;

public sealed class MotionSystem
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Version { get; set; } = "1.0.0";

    public string Description { get; set; } = string.Empty;

    public List<string> Personality { get; set; } = [];

    public List<MotionPrinciple> Principles { get; set; } = [];

    public MotionDurations Durations { get; set; } = new();

    public MotionEasings Easings { get; set; } = new();

    public MotionDistances Distances { get; set; } = new();

    public MotionScale Scale { get; set; } = new();

    public MotionOpacity Opacity { get; set; } = new();

    public MotionStagger Stagger { get; set; } = new();

    public MotionSemanticTokens Semantic { get; set; } = new();

    public MotionAccessibility Accessibility { get; set; } = new();
}
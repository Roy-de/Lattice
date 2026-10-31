using Lattice.Shared.DesignSystem.Motion.Models;

namespace Lattice.Shared.DesignSystem.Motion;

public interface IMotionCssGenerator
{
    string Generate(MotionSystem motion);
    string GenerateAll(IEnumerable<MotionSystem> motions);
}

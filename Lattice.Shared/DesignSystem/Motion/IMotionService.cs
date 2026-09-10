using Lattice.Shared.DesignSystem.Motion.Models;

namespace Lattice.Shared.DesignSystem.Motion;

public interface IMotionService
{
    Task<MotionSystem> LoadAsync(string motionId);
    
    Task<IEnumerable<MotionSystem>> LoadMultipleAsync(IEnumerable<string> motionIds);
    
    IReadOnlyCollection<MotionSystem> MotionSystems { get; }

    MotionSystem? GetMotionSystem(string motionId);
    
    IReadOnlyList<MotionSystem> getMotionSystemsSorted();

    void SwitchMotionSystem(string motionId);

    MotionSystem? Current { get; }

}
using System.Text.Json;
using Lattice.Shared.DesignSystem.Motion.Models;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Motion;

public sealed class MotionService: IMotionService
{
    private readonly IResourceLoader _resources;
    private readonly ILogger<MotionService> _logger;
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public MotionService(IResourceLoader resources, ILogger<MotionService> logger)
    {
        _resources = resources;
        _logger = logger;
    }

    public async Task<MotionSystem> LoadAsync(string motionId)
    {
        var path = $"Motion/{motionId}.json";
        try
        {
            await using var stream = await _resources.OpenAsync("Colors", path);
            using var reader = new StreamReader(stream, leaveOpen: true);
            
            var json = await reader.ReadToEndAsync();
            
            var motion = JsonSerializer.Deserialize<MotionSystem>(json, _options)
                ?? throw new InvalidOperationException($"Cannot deserialize motion 'motionId'");
        }
    }

    public Task<IEnumerable<MotionSystem>> LoadMultipleAsync(IEnumerable<string> motionIds)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<MotionSystem> MotionSystems { get; }
    public MotionSystem? GetMotionSystem(string motionId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<MotionSystem> getMotionSystemsSorted()
    {
        throw new NotImplementedException();
    }

    public void SwitchMotionSystem(string motionId)
    {
        throw new NotImplementedException();
    }

    public MotionSystem? Current { get; }
}
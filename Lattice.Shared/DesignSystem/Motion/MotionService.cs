using System.Text.Json;
using Lattice.Shared.DesignSystem.Motion.Models;
using Lattice.Shared.Resource;
using Microsoft.Extensions.Logging;

namespace Lattice.Shared.DesignSystem.Motion;

public sealed class MotionService : IMotionService
{
    private readonly IResourceLoader _resources;
    private readonly ILogger<MotionService> _logger;
    private readonly IMotionCssGenerator _motionCssGenerator;

    private readonly Dictionary<string, MotionSystem> _motionSystems =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private IReadOnlyCollection<MotionSystem> _motionSystemsSnapshot =
        Array.Empty<MotionSystem>();

    public MotionService(
        IResourceLoader resources,
        ILogger<MotionService> logger,
        IMotionCssGenerator motionCssGenerator)
    {
        _resources = resources
                     ?? throw new ArgumentNullException(nameof(resources));

        _logger = logger
                  ?? throw new ArgumentNullException(nameof(logger));

        _motionCssGenerator = motionCssGenerator
                              ?? throw new ArgumentNullException(nameof(motionCssGenerator));
    }

    public IReadOnlyCollection<MotionSystem> MotionSystems =>
        _motionSystemsSnapshot;

    public MotionSystem? Current { get; private set; }

    public string MotionCss { get; private set; } = string.Empty;

    public async Task<MotionSystem> LoadAsync(string motionId)
    {
        if (string.IsNullOrWhiteSpace(motionId))
        {
            throw new ArgumentException(
                "Motion ID cannot be null or empty.",
                nameof(motionId));
        }

        var normalizedId = motionId.Trim();

        var path = $"{normalizedId}.json";

        try
        {
            await using var stream =
                await _resources.OpenAsync("Motion", path);

            using var reader =
                new StreamReader(stream, leaveOpen: true);

            var json = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new InvalidOperationException(
                    $"Motion resource '{path}' is empty.");
            }

            var motion =
                JsonSerializer.Deserialize<MotionSystem>(
                    json,
                    _options);

            if (motion is null)
            {
                throw new InvalidOperationException(
                    $"Cannot deserialize motion '{normalizedId}'.");
            }

            motion.Id = normalizedId;

            _motionSystems[normalizedId] = motion;

            if (Current is null)
            {
                Current = motion;
            }

            MotionCss = _motionCssGenerator.Generate(motion);

            RefreshSnapshot();

            _logger.LogInformation(
                "Loaded motion {MotionId}",
                motion.Id);

            return motion;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error loading motion {MotionId}",
                normalizedId);

            throw;
        }
    }

    public async Task<IEnumerable<MotionSystem>> LoadMultipleAsync(
        IEnumerable<string> motionIds)
    {
        ArgumentNullException.ThrowIfNull(motionIds);

        var motions = new List<MotionSystem>();

        foreach (var motionId in motionIds)
        {
            if (string.IsNullOrWhiteSpace(motionId))
            {
                continue;
            }

            try
            {
                var motion = await LoadAsync(motionId);
                motions.Add(motion);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading motion {MotionId}",
                    motionId);
            }
        }

        return motions;
    }

    public MotionSystem? GetMotionSystem(string motionId)
    {
        if (string.IsNullOrWhiteSpace(motionId))
        {
            return null;
        }

        return _motionSystems.GetValueOrDefault(motionId.Trim());
    }

    public IReadOnlyList<MotionSystem> GetMotionSystemsSorted()
    {
        return _motionSystems.Values
            .OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public void SwitchMotionSystem(string motionId)
    {
        if (string.IsNullOrWhiteSpace(motionId))
        {
            throw new ArgumentException(
                "Motion ID cannot be null or empty.",
                nameof(motionId));
        }

        var motion = GetMotionSystem(motionId);

        if (motion is null)
        {
            throw new KeyNotFoundException(
                $"Motion not found for id '{motionId}'.");
        }

        Current = motion;

        MotionCss = _motionCssGenerator.Generate(motion);
    }

    private void RefreshSnapshot()
    {
        _motionSystemsSnapshot = _motionSystems.Values.ToList();
    }
}
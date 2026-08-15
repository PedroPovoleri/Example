using Microsoft.Extensions.Logging;
using ResumeHexagonal.Domain;
using ResumeHexagonal.Domain.Ports;

namespace ResumeHexagonal.Infrastructure;

/// <summary>
/// In-memory adapter for creating and persisting resume events.
/// This is an outbound adapter (right side of hexagon) for event creation.
/// </summary>
public sealed class MemoryForCreateEvent(Dictionary<Guid, ResumeEvent>? store = null) : ForCreateEvent
{
    private readonly Dictionary<Guid, ResumeEvent> _store = store ?? new();

    /// <summary>
    /// Creates and stores a resume event in memory.
    /// </summary>
    public Task<ResumeEvent> CreateAsync(ResumeEvent resumeEvent, CancellationToken cancellationToken = default)
    {
        _store[resumeEvent.Id] = resumeEvent;
        return Task.FromResult(resumeEvent);
    }
}

/// <summary>
/// In-memory adapter for reading resume events.
/// This is an outbound adapter (right side of hexagon) for event retrieval.
/// </summary>
public sealed class MemoryForReadEvent(Dictionary<Guid, ResumeEvent>? store = null) : ForReadEvent
{
    private readonly Dictionary<Guid, ResumeEvent> _store = store ?? new();

    /// <summary>
    /// Retrieves a resume event from memory by its identifier.
    /// </summary>
    public Task<ResumeEvent?> ReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.TryGetValue(id, out var resumeEvent) ? resumeEvent : null);
    }
}

/// <summary>
/// In-memory adapter for logging resume events.
/// This is an outbound adapter (right side of hexagon) for event logging.
/// Stores messages in memory; in production, inject ILogger for real logging.
/// </summary>
public sealed class MemoryForLogEvent(ILogger<MemoryForLogEvent> logger) : ForLogEvent
{
    private readonly ILogger<MemoryForLogEvent> _logger = logger;
    private readonly List<string> _messages = [];

    /// <summary>
    /// Gets all logged messages stored in memory.
    /// </summary>
    public IReadOnlyList<string> Messages => _messages;

    /// <summary>
    /// Logs a message using the provided logger and stores it in memory.
    /// </summary>
    public Task LogAsync(string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(message);
        _messages.Add(message);
        return Task.CompletedTask;
    }
}

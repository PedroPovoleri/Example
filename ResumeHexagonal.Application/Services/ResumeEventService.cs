using ResumeHexagonal.Domain;
using ResumeHexagonal.Domain.Ports;

namespace ResumeHexagonal.Application.Services;

/// <summary>
/// Application service for managing resume events using the hexagonal architecture pattern.
/// This service acts as the use-case orchestrator, coordinating between domain models and ports.
/// </summary>
public sealed class ResumeEventService(
    ForCreateEvent createEvent,
    ForReadEvent readEvent,
    ForLogEvent logEvent)
{
    private readonly ForCreateEvent _createEvent = createEvent;
    private readonly ForReadEvent _readEvent = readEvent;
    private readonly ForLogEvent _logEvent = logEvent;

    /// <summary>
    /// Creates and persists a new resume event.
    /// </summary>
    /// <param name="title">The event title (required, non-empty)</param>
    /// <param name="description">The event description (required, non-empty)</param>
    /// <param name="resource">The resource type (required, non-empty)</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>The created resume event</returns>
    /// <exception cref="ArgumentException">Thrown when any required parameter is null or empty</exception>
    public async Task<ResumeEvent> CreateAsync(
        string title,
        string description,
        string resource,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);

        var entity = new ResumeEvent(Guid.NewGuid(), title.Trim(), description.Trim(), resource.Trim(), DateTime.UtcNow);
        var created = await _createEvent.CreateAsync(entity, cancellationToken);

        await _logEvent.LogAsync($"created {created.Resource} event: {created.Title}", cancellationToken);
        return created;
    }

    /// <summary>
    /// Retrieves a resume event by its identifier.
    /// </summary>
    /// <param name="id">The event identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>The resume event if found; otherwise null</returns>
    public async Task<ResumeEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _readEvent.ReadAsync(id, cancellationToken);

        await _logEvent.LogAsync($"read {result?.Resource ?? "unknown"} event: {id}", cancellationToken);
        return result;
    }
}

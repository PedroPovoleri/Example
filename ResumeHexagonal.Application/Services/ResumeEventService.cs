using ResumeHexagonal.Domain;
using ResumeHexagonal.Domain.Ports;

namespace ResumeHexagonal.Application.Services;

public sealed class ResumeEventService(
    ForCreateEvent createEvent,
    ForReadEvent readEvent,
    ForLogEvent logEvent)
{
    private readonly ForCreateEvent _createEvent = createEvent;
    private readonly ForReadEvent _readEvent = readEvent;
    private readonly ForLogEvent _logEvent = logEvent;

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

    public async Task<ResumeEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _readEvent.ReadAsync(id, cancellationToken);

        await _logEvent.LogAsync($"read {result?.Resource ?? "unknown"} event: {id}", cancellationToken);
        return result;
    }
}

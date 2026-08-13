using ResumeHexagonal.Domain;
using ResumeHexagonal.Domain.Ports;

namespace ResumeHexagonal.Infrastructure;

public sealed class MemoryForCreateEvent(Dictionary<Guid, ResumeEvent>? store = null) : ForCreateEvent
{
    private readonly Dictionary<Guid, ResumeEvent> _store = store ?? new();

    public Task<ResumeEvent> CreateAsync(ResumeEvent resumeEvent, CancellationToken cancellationToken = default)
    {
        _store[resumeEvent.Id] = resumeEvent;
        return Task.FromResult(resumeEvent);
    }
}

public sealed class MemoryForReadEvent(Dictionary<Guid, ResumeEvent>? store = null) : ForReadEvent
{
    private readonly Dictionary<Guid, ResumeEvent> _store = store ?? new();

    public Task<ResumeEvent?> ReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.TryGetValue(id, out var resumeEvent) ? resumeEvent : null);
    }
}

public sealed class MemoryForLogEvent : ForLogEvent
{
    private readonly List<string> _messages = [];

    public IReadOnlyList<string> Messages => _messages;

    public Task LogAsync(string message, CancellationToken cancellationToken = default)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }
}

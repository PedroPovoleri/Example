using ResumeHexagonal.Application.Services;
using ResumeHexagonal.Domain;
using ResumeHexagonal.Domain.Ports;

namespace ResumeHexagonal.Tests;

public class ResumeEventServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistAndLogEvent()
    {
        var store = new Dictionary<Guid, ResumeEvent>();
        var create = new RecordingForCreateEvent(store);
        var read = new RecordingForReadEvent(store);
        var log = new RecordingForLogEvent();
        var service = new ResumeEventService(create, read, log);

        var result = await service.CreateAsync("Senior backend engineer", "Strong .NET and Azure profile", "resume");

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Senior backend engineer", result.Title);
        Assert.Single(create.Created);
        Assert.Contains("created resume event", log.Messages.Single());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReadFromDrivenPortAndLogAccess()
    {
        var store = new Dictionary<Guid, ResumeEvent>();
        var create = new RecordingForCreateEvent(store);
        var read = new RecordingForReadEvent(store);
        var log = new RecordingForLogEvent();
        var service = new ResumeEventService(create, read, log);

        var created = await service.CreateAsync("Role fit", "Energy and finance backend work", "resume");
        var loaded = await service.GetByIdAsync(created.Id);

        Assert.NotNull(loaded);
        Assert.Equal(created.Id, loaded!.Id);
        Assert.Equal(2, log.Messages.Count);
    }

    private sealed class RecordingForCreateEvent(Dictionary<Guid, ResumeEvent> store) : ForCreateEvent
    {
        public List<ResumeEvent> Created { get; } = [];

        public Task<ResumeEvent> CreateAsync(ResumeEvent resumeEvent, CancellationToken cancellationToken = default)
        {
            store[resumeEvent.Id] = resumeEvent;
            Created.Add(resumeEvent);
            return Task.FromResult(resumeEvent);
        }
    }

    private sealed class RecordingForReadEvent(Dictionary<Guid, ResumeEvent> store) : ForReadEvent
    {
        public Task<ResumeEvent?> ReadAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(store.TryGetValue(id, out var eventItem) ? eventItem : null);
        }
    }

    private sealed class RecordingForLogEvent : ForLogEvent
    {
        public List<string> Messages { get; } = [];

        public Task LogAsync(string message, CancellationToken cancellationToken = default)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}

namespace ResumeHexagonal.Domain.Ports;

public interface ForCreateEvent
{
    Task<ResumeEvent> CreateAsync(ResumeEvent resumeEvent, CancellationToken cancellationToken = default);
}

namespace ResumeHexagonal.Domain.Ports;

public interface ForReadEvent
{
    Task<ResumeEvent?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}

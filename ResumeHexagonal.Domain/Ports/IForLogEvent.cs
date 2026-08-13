namespace ResumeHexagonal.Domain.Ports;

public interface ForLogEvent
{
    Task LogAsync(string message, CancellationToken cancellationToken = default);
}

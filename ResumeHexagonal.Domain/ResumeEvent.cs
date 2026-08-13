namespace ResumeHexagonal.Domain;

public sealed class ResumeEvent(
    Guid id,
    string title,
    string description,
    string resource,
    DateTime createdAt)
{
    public Guid Id { get; } = id;
    public string Title { get; } = title;
    public string Description { get; } = description;
    public string Resource { get; } = resource;
    public DateTime CreatedAt { get; } = createdAt;
}

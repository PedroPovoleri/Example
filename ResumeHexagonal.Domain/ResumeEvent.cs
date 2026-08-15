namespace ResumeHexagonal.Domain;

/// <summary>
/// Represents a resume event in the domain model.
/// </summary>
public sealed class ResumeEvent(
    Guid id,
    string title,
    string description,
    string resource,
    DateTime createdAt)
{
    /// <summary>
    /// Unique identifier for the resume event.
    /// </summary>
    public Guid Id { get; } = id;
    
    /// <summary>
    /// Title of the resume event.
    /// </summary>
    public string Title { get; } = title;
    
    /// <summary>
    /// Detailed description of the resume event.
    /// </summary>
    public string Description { get; } = description;
    
    /// <summary>
    /// Resource type associated with the event (e.g., 'resume', 'portfolio').
    /// </summary>
    public string Resource { get; } = resource;
    
    /// <summary>
    /// Timestamp when the event was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; } = createdAt;
}

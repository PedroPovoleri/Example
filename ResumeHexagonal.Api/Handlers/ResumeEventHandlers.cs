using System.ComponentModel.DataAnnotations;
using ResumeHexagonal.Application.Services;
using ResumeHexagonal.Domain;

namespace ResumeHexagonal.Api.Handlers;

/// <summary>
/// HTTP handlers for resume event management endpoints.
/// </summary>
public static class ResumeEventHandlers
{
    /// <summary>
    /// Creates a new resume event.
    /// </summary>
    public static async Task<IResult> CreateAsync(
        CreateResumeEventRequest request,
        ResumeEventService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request.Title, request.Description, request.Resource, cancellationToken);
        return Results.Created($"/api/resume-events/{result.Id}", result);
    }

    /// <summary>
    /// Retrieves a resume event by its identifier.
    /// </summary>
    public static async Task<IResult> GetByIdAsync(
        Guid id,
        ResumeEventService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}

/// <summary>
/// Request to create a new resume event.
/// </summary>
public sealed record CreateResumeEventRequest(
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(255, ErrorMessage = "Title must not exceed 255 characters.")]
    string Title,
    
    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
    string Description,
    
    [Required(ErrorMessage = "Resource is required.")]
    string Resource);

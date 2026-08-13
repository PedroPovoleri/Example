using ResumeHexagonal.Application.Services;
using ResumeHexagonal.Domain;

namespace ResumeHexagonal.Api.Handlers;

public static class ResumeEventHandlers
{
    public static async Task<IResult> CreateAsync(
        CreateResumeEventRequest request,
        ResumeEventService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request.Title, request.Description, request.Resource, cancellationToken);
        return Results.Ok(result);
    }

    public static async Task<IResult> GetByIdAsync(
        Guid id,
        ResumeEventService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}

public sealed record CreateResumeEventRequest(
    string Title,
    string Description,
    string Resource);

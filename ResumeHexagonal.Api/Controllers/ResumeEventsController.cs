using Microsoft.AspNetCore.Mvc;
using ResumeHexagonal.Application.Services;
using ResumeHexagonal.Domain;

namespace ResumeHexagonal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResumeEventsController : ControllerBase
{
    private readonly ResumeEventService _service;

    public ResumeEventsController(ResumeEventService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ResumeEvent>> Create([FromBody] CreateResumeEventRequest request)
    {
        var result = await _service.CreateAsync(request.Title, request.Description, request.Resource);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResumeEvent>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}

public sealed class CreateResumeEventRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
}

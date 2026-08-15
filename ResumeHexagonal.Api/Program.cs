using ResumeHexagonal.Api;
using ResumeHexagonal.Api.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddResumeHexagonal();

var app = builder.Build();

// Global exception handling
app.UseExceptionHandler(exceptionHandlerApp => 
{
    exceptionHandlerApp.Run(async context => 
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var group = app.MapGroup("/api/resume-events");

group.MapPost("", ResumeEventHandlers.CreateAsync)
    .WithName("CreateResumeEvent")
    .Produces<ResumeEvent>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

group.MapGet("{id:guid}", ResumeEventHandlers.GetByIdAsync)
    .WithName("GetResumeEventById")
    .Produces<ResumeEvent>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.Run();

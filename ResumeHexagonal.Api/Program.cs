using ResumeHexagonal.Api;
using ResumeHexagonal.Api.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddResumeHexagonal();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var group = app.MapGroup("/api/resume-events");

group.MapPost("", ResumeEventHandlers.CreateAsync)
    .WithName("CreateResumeEvent");

group.MapGet("{id:guid}", ResumeEventHandlers.GetByIdAsync)
    .WithName("GetResumeEventById");

app.Run();

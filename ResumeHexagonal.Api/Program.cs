using ResumeHexagonal.Api;
using ResumeHexagonal.Api.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddResumeHexagonal();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var group = app.MapGroup("/api/resume-events");

group.MapPost("", ResumeEventHandlers.CreateAsync)
    .WithName("CreateResumeEvent")
    .WithOpenApi();

group.MapGet("{id:guid}", ResumeEventHandlers.GetByIdAsync)
    .WithName("GetResumeEventById")
    .WithOpenApi();

app.Run();

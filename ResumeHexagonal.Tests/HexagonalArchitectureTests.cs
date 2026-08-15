using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using ResumeHexagonal.Api;
using ResumeHexagonal.Api.Handlers;
using ResumeHexagonal.Application.Services;
using ResumeHexagonal.Domain;
using ResumeHexagonal.Domain.Ports;
using ResumeHexagonal.Infrastructure;

namespace ResumeHexagonal.Tests;

public class HexagonalArchitectureTests
{
    private readonly ILogger<MemoryForLogEvent> _logger;

    public HexagonalArchitectureTests(ITestOutputHelper output)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXunit(output));
        var provider = services.BuildServiceProvider();
        _logger = provider.GetRequiredService<ILogger<MemoryForLogEvent>>();
    }

    [Fact]
    public void DependencyInjection_ShouldRegisterAllPortsAndService()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddResumeHexagonal();

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<ForCreateEvent>());
        Assert.NotNull(provider.GetService<ForReadEvent>());
        Assert.NotNull(provider.GetService<ForLogEvent>());
        Assert.NotNull(provider.GetService<ResumeEventService>());
    }

    [Fact]
    public async Task MemoryDrivenAdapters_ShouldPersistAndReadEvents()
    {
        var store = new Dictionary<Guid, ResumeEvent>();
        var create = new MemoryForCreateEvent(store);
        var read = new MemoryForReadEvent(store);
        var log = new MemoryForLogEvent(_logger);
        var service = new ResumeEventService(create, read, log);

        var created = await service.CreateAsync("Senior .NET Engineer", "Cloud, DDD, and Azure", "resume");
        var loaded = await service.GetByIdAsync(created.Id);

        Assert.Equal(created.Id, loaded!.Id);
        Assert.Equal("Senior .NET Engineer", loaded.Title);
        Assert.Contains(log.Messages, x => x.Contains("created resume event"));
    }

    [Fact]
    public async Task CreateHandler_ShouldReturnCreatedWithCreatedEvent()
    {
        var store = new Dictionary<Guid, ResumeEvent>();
        var service = new ResumeEventService(new MemoryForCreateEvent(store), new MemoryForReadEvent(store), new MemoryForLogEvent(_logger));
        var request = new CreateResumeEventRequest("Cloud Engineer", "Azure and event-driven systems", "resume");

        IResult result = await ResumeEventHandlers.CreateAsync(request, service, CancellationToken.None);

        var created = result as Created<ResumeEvent>;

        Assert.NotNull(created);
        Assert.Equal("Cloud Engineer", created!.Value!.Title);
        Assert.Equal("resume", created.Value.Resource);
    }

    [Fact]
    public async Task GetByIdHandler_ShouldReturnNotFound_WhenEventDoesNotExist()
    {
        var store = new Dictionary<Guid, ResumeEvent>();
        var service = new ResumeEventService(new MemoryForCreateEvent(store), new MemoryForReadEvent(store), new MemoryForLogEvent(_logger));

        IResult result = await ResumeEventHandlers.GetByIdAsync(Guid.NewGuid(), service, CancellationToken.None);

        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task Service_ShouldRejectEmptyRequiredValues()
    {
        var service = new ResumeEventService(new MemoryForCreateEvent(), new MemoryForReadEvent(), new MemoryForLogEvent(_logger));

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(" ", "desc", "resume"));
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync("title", " ", "resume"));
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync("title", "desc", " "));
    }
}

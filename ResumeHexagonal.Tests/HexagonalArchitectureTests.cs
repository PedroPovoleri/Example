using Microsoft.Extensions.DependencyInjection;
using ResumeHexagonal.Api;
using ResumeHexagonal.Api.Handlers;
using ResumeHexagonal.Application.Services;
using ResumeHexagonal.Domain;
using ResumeHexagonal.Domain.Ports;
using ResumeHexagonal.Infrastructure;

namespace ResumeHexagonal.Tests;

public class HexagonalArchitectureTests
{
    [Fact]
    public void DependencyInjection_ShouldRegisterAllPortsAndService()
    {
        var services = new ServiceCollection();

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
        var log = new MemoryForLogEvent();
        var service = new ResumeEventService(create, read, log);

        var created = await service.CreateAsync("Senior .NET Engineer", "Cloud, DDD, and Azure", "resume");
        var loaded = await service.GetByIdAsync(created.Id);

        Assert.Equal(created.Id, loaded!.Id);
        Assert.Equal("Senior .NET Engineer", loaded.Title);
        Assert.Contains(log.Messages, x => x.Contains("created resume event"));
    }

    [Fact]
    public async Task CreateHandler_ShouldReturnOkWithCreatedEvent()
    {
        var store = new Dictionary<Guid, ResumeEvent>();
        var service = new ResumeEventService(new MemoryForCreateEvent(store), new MemoryForReadEvent(store), new MemoryForLogEvent());
        var request = new CreateResumeEventRequest("Cloud Engineer", "Azure and event-driven systems", "resume");

        IResult result = await ResumeEventHandlers.CreateAsync(request, service, CancellationToken.None);

        var ok = result as Ok<ResumeEvent>;

        Assert.NotNull(ok);
        Assert.Equal("Cloud Engineer", ok!.Value!.Title);
        Assert.Equal("resume", ok.Value.Resource);
    }

    [Fact]
    public async Task GetByIdHandler_ShouldReturnNotFound_WhenEventDoesNotExist()
    {
        var store = new Dictionary<Guid, ResumeEvent>();
        var service = new ResumeEventService(new MemoryForCreateEvent(store), new MemoryForReadEvent(store), new MemoryForLogEvent());

        IResult result = await ResumeEventHandlers.GetByIdAsync(Guid.NewGuid(), service, CancellationToken.None);

        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task Service_ShouldRejectEmptyRequiredValues()
    {
        var service = new ResumeEventService(new MemoryForCreateEvent(), new MemoryForReadEvent(), new MemoryForLogEvent());

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(" ", "desc", "resume"));
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync("title", " ", "resume"));
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync("title", "desc", " "));
    }
}

using ResumeHexagonal.Application.Services;
using ResumeHexagonal.Domain;
using ResumeHexagonal.Domain.Ports;
using ResumeHexagonal.Infrastructure;

namespace ResumeHexagonal.Api;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the hexagonal architecture components for resume event management.
    /// </summary>
    public static IServiceCollection AddResumeHexagonal(this IServiceCollection services)
    {
        var eventStore = new Dictionary<Guid, ResumeEvent>();

        // Register shared event store
        services.AddSingleton(eventStore);
        
        // Register ports (adapters)
        services.AddScoped<ForCreateEvent>(sp => new MemoryForCreateEvent(sp.GetRequiredService<Dictionary<Guid, ResumeEvent>>()));
        services.AddScoped<ForReadEvent>(sp => new MemoryForReadEvent(sp.GetRequiredService<Dictionary<Guid, ResumeEvent>>()));
        services.AddScoped<ForLogEvent>(sp => new MemoryForLogEvent(sp.GetRequiredService<ILogger<MemoryForLogEvent>>()));
        
        // Register application service
        services.AddScoped<ResumeEventService>();

        return services;
    }
}

using ResumeHexagonal.Application.Services;
using ResumeHexagonal.Domain;
using ResumeHexagonal.Domain.Ports;
using ResumeHexagonal.Infrastructure;

namespace ResumeHexagonal.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddResumeHexagonal(this IServiceCollection services)
    {
        var eventStore = new Dictionary<Guid, ResumeEvent>();

        services.AddSingleton(eventStore);
        services.AddScoped<ForCreateEvent>(sp => new MemoryForCreateEvent(sp.GetRequiredService<Dictionary<Guid, ResumeEvent>>()));
        services.AddScoped<ForReadEvent>(sp => new MemoryForReadEvent(sp.GetRequiredService<Dictionary<Guid, ResumeEvent>>()));
        services.AddScoped<ForLogEvent, MemoryForLogEvent>();
        services.AddScoped<ResumeEventService>();

        return services;
    }
}

namespace UniversalDevice;

public interface ITelemetrySource
{
    Task<Telemetry> NextAsync(CancellationToken cancellationToken = default);
}

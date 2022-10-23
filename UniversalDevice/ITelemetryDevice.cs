namespace UniversalDevice;

public interface ITelemetryDevice
{
    TimeSpan TelemetryInterval { get; set; }
}

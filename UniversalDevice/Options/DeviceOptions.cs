using Microsoft.Azure.Devices.Client;

namespace UniversalDevice.Options;

public sealed class DeviceOptions
{
    public TransportType TransportType { get; set; }
    public string? DeviceConnectionString { get; set; }
    public string? ModelId { get; set; }
}

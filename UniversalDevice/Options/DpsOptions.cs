using Microsoft.Azure.Devices.Client;

namespace UniversalDevice.Options;

#nullable disable warnings
public sealed class DpsOptions
{
    public string GlobalDeviceEndpoint { get; set; }
    public string IdScope { get; set; }
    public string RegistrationId { get; set; }
    public string PrimaryKey { get; set; }
    public string? SecondaryKey { get; set; }
    public TransportType TransportType { get; set; }
}

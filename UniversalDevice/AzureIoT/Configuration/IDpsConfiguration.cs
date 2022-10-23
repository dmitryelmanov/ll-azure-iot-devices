using Microsoft.Azure.Devices.Client;

namespace UniversalDevice.AzureIoT.Configuration;

public interface IDpsConfiguration
{
    string GlobalDeviceEndpoint { get; set; }
    string IdScope { get; set; }
    string RegistrationId { get; set; }
    string PrimaryKey { get; set; }
    string? SecondaryKey { get; set; }
    TransportType TransportType { get; set; }
}

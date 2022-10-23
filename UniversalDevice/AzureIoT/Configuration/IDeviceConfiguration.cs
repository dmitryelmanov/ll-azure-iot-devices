using Microsoft.Azure.Devices.Client;

namespace UniversalDevice.AzureIoT.Configuration;

public interface IDeviceConfiguration
{
    TransportType TransportType { get; set; }
}

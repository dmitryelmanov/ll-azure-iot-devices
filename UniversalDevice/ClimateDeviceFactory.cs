using Microsoft.Extensions.Logging;
using UniversalDevice.AzureIoT.Configuration;
using UniversalDevice.Options;

namespace UniversalDevice;

internal class ClimateDeviceFactory
{
    public static ClimateDevice Create(
        DeviceOptions deviceOptions,
        DpsOptions? dpsOptions,
        EdgeDeviceOptions? edgeDeviceOptions,
        ILogger? logger = null)
        => dpsOptions != null
        ? new ClimateDevice(
            new ProvisionAndConnectConfiguration
            {
                GlobalDeviceEndpoint = dpsOptions.GlobalDeviceEndpoint,
                IdScope = dpsOptions.IdScope,
                PrimaryKey = dpsOptions.PrimaryKey,
                SecondaryKey = dpsOptions.SecondaryKey,
                ProvisioningTransportType = dpsOptions.TransportType,
                RegistrationId = dpsOptions.RegistrationId,
                ConnectingTransportType = deviceOptions.TransportType,
                HostName = edgeDeviceOptions?.HostName,
                ModelId = deviceOptions.ModelId,
            },
            logger)
        : new ClimateDevice(
            new ConnectConfiguration
            {
                DeviceConnectionString = deviceOptions.DeviceConnectionString!,
                TransportType = deviceOptions.TransportType,
                ModelId = deviceOptions.ModelId,
            },
            logger);
}

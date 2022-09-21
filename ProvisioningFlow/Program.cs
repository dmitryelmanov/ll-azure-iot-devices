using System.Text;
using Common;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;

const string GLOBAL_DEVICE_ENDPOINT = "<DPS_Global_Device_Endpoint>";
const string ID_SCOPE = "<DPS_ID_Scope>";
const string REGISTRATION_ID = "<DPS_Enrollement_Registration_Id>";
const string KEY = "<DPS_Enrollment_Primary_Key>";
const int TELEMETRY_INTERVAL = 3 * 1000;

ConsoleWriter.Write("Creating provisioning client... ", ConsoleColor.Cyan);
using var security = new SecurityProviderSymmetricKey(REGISTRATION_ID, KEY, null);
using var transport = new ProvisioningTransportHandlerMqtt();
var provisioningClient = ProvisioningDeviceClient.Create(GLOBAL_DEVICE_ENDPOINT, ID_SCOPE, security, transport);
ConsoleWriter.WriteLine("OK", ConsoleColor.Green);

ConsoleWriter.Write("Registering with the device provisioning service...", ConsoleColor.Cyan);
var result = await provisioningClient.RegisterAsync();
ConsoleWriter.WriteLine($"{result.Status}", result.Status == ProvisioningRegistrationStatusType.Assigned
    ? ConsoleColor.Green
    : ConsoleColor.Red);

if (result.Status != ProvisioningRegistrationStatusType.Assigned)
{
    ConsoleWriter.WriteLine($"Registration status did not assign a hub, so exiting this sample.", ConsoleColor.Red);
    return;
}

ConsoleWriter.WriteLine($"Device {result.DeviceId} registered to {result.AssignedHub}.", ConsoleColor.Green);

ConsoleWriter.WriteLine("Creating symmetric key authentication for IoT Hub...", ConsoleColor.Cyan);
var auth = new DeviceAuthenticationWithRegistrySymmetricKey(result.DeviceId, security.GetPrimaryKey());

ConsoleWriter.Write("Creating device client... ", ConsoleColor.Cyan);
using var deviceClient = DeviceClient.Create(result.AssignedHub, auth, TransportType.Mqtt);
ConsoleWriter.WriteLine("OK", ConsoleColor.Green);

ConsoleWriter.WriteLine("Sending telemetry", ConsoleColor.Cyan);
await Loop.ExecuteAsync(async () =>
{
    var data = $"Current Time is: {DateTime.Now}";
    var message = new Message(Encoding.UTF8.GetBytes(data));
    await deviceClient.SendEventAsync(message);
},
new Loop.Options { IntervalMs = TELEMETRY_INTERVAL });

ConsoleWriter.WriteLine("Close device connection", ConsoleColor.Cyan);
await deviceClient.CloseAsync();

ConsoleWriter.WriteLine($"Stop IoT Device {result.DeviceId}", ConsoleColor.Cyan);

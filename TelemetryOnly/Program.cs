using System.Text;
using Common;
using Microsoft.Azure.Devices.Client;

const string DEVICE_CONNECTION_STRING = "<Device's_Primary_Connection_String>";
const int TELEMETRY_INTERVAL = 3 * 1000;

Console.WriteLine($"Start IoT Device {DEVICE_CONNECTION_STRING.GetDeviceId()}");

Console.WriteLine("Create device client");
using var client = DeviceClient.CreateFromConnectionString(DEVICE_CONNECTION_STRING, TransportType.Mqtt);

Console.WriteLine("Sending telemetry");
await Loop.ExecuteAsync(async () =>
{
    var data = $"Current Time is: {DateTime.Now}";
    var message = new Message(Encoding.UTF8.GetBytes(data));
    await client.SendEventAsync(message);
},
new Loop.Options { IntervalMs = TELEMETRY_INTERVAL });

Console.WriteLine("Close device connection");
await client.CloseAsync();

Console.WriteLine($"Stop IoT Device {DEVICE_CONNECTION_STRING.GetDeviceId()}");
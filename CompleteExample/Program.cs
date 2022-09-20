using System.Text;
using Common;
using CompleteExample;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

const string DEVICE_CONNECTION_STRING = "<Device's_Primary_Connection_String>";
var execOptions = new Loop.Options
{
    IntervalMs = 3 * 1000,
};

ConsoleWriter.WriteLine($"Start IoT Device {DEVICE_CONNECTION_STRING.GetDeviceId()}", ConsoleColor.Cyan);

ConsoleWriter.Write("Creating device client... ", ConsoleColor.Cyan);
using var client = DeviceClient.CreateFromConnectionString(DEVICE_CONNECTION_STRING, TransportType.Mqtt);
ConsoleWriter.WriteLine("OK", ConsoleColor.Green);

ConsoleWriter.WriteLine("Setup Connection Status Handler", ConsoleColor.Cyan);
client.SetConnectionStatusChangesHandler(async (status, reason) =>
{
    ConsoleWriter.WriteLine($"Connection status changed to {status} because of {reason}", ConsoleColor.Yellow);

    if (status == ConnectionStatus.Connected)
    {
        ConsoleWriter.WriteLine("Sync properties", ConsoleColor.Cyan);
        ConsoleWriter.WriteLine("Get device Twin", ConsoleColor.Cyan);
        var twin = await client.GetTwinAsync();
        ConsoleWriter.WriteLine("Twin:", ConsoleColor.Cyan);
        ConsoleWriter.WriteLine(twin?.ToJson(Formatting.Indented), ConsoleColor.Blue);

        await UpdateProps(twin!.Properties.Desired);
    }
});

ConsoleWriter.WriteLine("Setup Desired Property Handlers", ConsoleColor.Cyan);
await client.SetDesiredPropertyUpdateCallbackAsync(
    async (props, userContext) =>
    {
        var reported = new TwinCollection();
        ConsoleWriter.WriteLine("Desired properties update received:", ConsoleColor.Yellow);
        await UpdateProps(props);
    },
    null);

ConsoleWriter.WriteLine("Setup Direct Methods Handlers", ConsoleColor.Cyan);
await client.SetMethodHandlerAsync(
    "ping",
    async (request, userContext) =>
    {
        ConsoleWriter.WriteLine($"Ping method executed with data:", ConsoleColor.Yellow);
        ConsoleWriter.WriteLine(Encoding.UTF8.GetString(request?.Data ?? Array.Empty<byte>()), ConsoleColor.Magenta);
        return new MethodResponse(Encoding.UTF8.GetBytes("{ \"response\": \"pong\" }"), 200);
    },
    null);

ConsoleWriter.WriteLine("Setup C2D Message Handler", ConsoleColor.Cyan);
await client.SetReceiveMessageHandlerAsync(
    async (msg, userContext) =>
    {
        using var reader = new StreamReader(msg.BodyStream);
        ConsoleWriter.WriteLine("C2D message received:", ConsoleColor.Yellow);
        ConsoleWriter.WriteLine(reader.ReadToEnd(), ConsoleColor.Magenta);
        await client.CompleteAsync(msg);
    },
    null);

ConsoleWriter.WriteLine("Sending telemetry", ConsoleColor.Cyan);
var source = new TelemetrySource();
await Loop.ExecuteAsync(async () =>
{
    var telemetry = source.Next();
    var json = JsonConvert.SerializeObject(telemetry);
    var message = new Message(Encoding.UTF8.GetBytes(json));
    await client.SendEventAsync(message);
},
execOptions);

ConsoleWriter.WriteLine("Close device connection", ConsoleColor.Cyan);
await client.CloseAsync();

ConsoleWriter.WriteLine($"Stop IoT Device {DEVICE_CONNECTION_STRING.GetDeviceId()}", ConsoleColor.Cyan);


async Task UpdateProps(TwinCollection desired)
{
    var reported = new TwinCollection();
    foreach (KeyValuePair<string, object> prop in desired)
    {
        ConsoleWriter.WriteLine($"{prop.Key}: {prop.Value}", ConsoleColor.Magenta);
        if (prop.Key == "TelemetryInterval")
        {
            execOptions!.IntervalMs = Convert.ToInt32(prop.Value);
            reported["TelemetryInterval"] = execOptions!.IntervalMs;
        }
        else
        {
            reported[prop.Key] = prop.Value;
        }
    }

    ConsoleWriter.WriteLine("Update reported properties", ConsoleColor.Cyan);
    await client!.UpdateReportedPropertiesAsync(reported);
    ConsoleWriter.WriteLine("Reported properties updated", ConsoleColor.Green);
}
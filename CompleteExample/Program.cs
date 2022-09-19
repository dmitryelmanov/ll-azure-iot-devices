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

ConsoleWriter.WriteLine($"Start IoT Device {DEVICE_CONNECTION_STRING.GetDeviceId()}", ConsoleColor.Green);

ConsoleWriter.WriteLine("Create device client", ConsoleColor.Green);
using var client = DeviceClient.CreateFromConnectionString(DEVICE_CONNECTION_STRING, TransportType.Mqtt);

ConsoleWriter.WriteLine("Setup Connection Status Handler", ConsoleColor.Green);
client.SetConnectionStatusChangesHandler(async (status, reason) =>
{
    ConsoleWriter.WriteLine($"Connection status changed to {status} because of {reason}", ConsoleColor.Yellow);

    if (status == ConnectionStatus.Connected)
    {
        ConsoleWriter.WriteLine("Sync properties", ConsoleColor.Green);
        var twin = await client.GetTwinAsync();
        ConsoleWriter.WriteLine("Get device Twin", ConsoleColor.Green);
        ConsoleWriter.WriteLine("Twin:", ConsoleColor.Green);
        ConsoleWriter.WriteLine(twin?.ToJson(), ConsoleColor.Blue);
        foreach (KeyValuePair<string, object> prop in twin!.Properties.Desired)
        {
            if (prop.Key == "TelemetryInterval")
            {
                ConsoleWriter.WriteLine($"Set 'TelemetryInterval' to {prop.Value}", ConsoleColor.Magenta);
                execOptions.IntervalMs = Convert.ToInt32(prop.Value);
            }
        }
        ConsoleWriter.WriteLine("Update reported properties", ConsoleColor.Green);
        var reported = new TwinCollection();
        reported["TelemetryInterval"] = execOptions.IntervalMs;
        await client.UpdateReportedPropertiesAsync(reported);
    }
});

ConsoleWriter.WriteLine("Setup Desired Property Handlers", ConsoleColor.Green);
await client.SetDesiredPropertyUpdateCallbackAsync(
    async (props, userContext) =>
    {
        var reported = new TwinCollection();
        ConsoleWriter.WriteLine("Desired properties update received:", ConsoleColor.Yellow);
        foreach (KeyValuePair<string, object> prop in props)
        {
            ConsoleWriter.WriteLine($"{prop.Key}: {prop.Value}", ConsoleColor.Magenta);
            switch (prop.Key)
            {
                case "TelemetryInterval":
                    execOptions.IntervalMs = Convert.ToInt32(prop.Value);
                    reported[prop.Key] = execOptions.IntervalMs;
                    break;
            }
        }

        await client.UpdateReportedPropertiesAsync(reported);
    },
    null);

ConsoleWriter.WriteLine("Setup Direct Methods", ConsoleColor.Green);
await client.SetMethodHandlerAsync(
    "ping",
    async (request, userContext) =>
    {
        ConsoleWriter.WriteLine($"Ping method executed with data:", ConsoleColor.Yellow);
        ConsoleWriter.WriteLine(Encoding.UTF8.GetString(request?.Data ?? Array.Empty<byte>()), ConsoleColor.Magenta);
        return new MethodResponse(Encoding.UTF8.GetBytes("{ \"response\": \"pong\" }"), 200);
    },
    null);

ConsoleWriter.WriteLine("Setup C2D Message Handler", ConsoleColor.Green);
await client.SetReceiveMessageHandlerAsync(
    async (msg, userContext) =>
    {
        using var reader = new StreamReader(msg.BodyStream);
        ConsoleWriter.WriteLine("C2D message received:", ConsoleColor.Yellow);
        ConsoleWriter.WriteLine(reader.ReadToEnd(), ConsoleColor.Magenta);
    },
    null);

ConsoleWriter.WriteLine("Sending telemetry", ConsoleColor.Green);
var source = new TelemetrySource();
await Loop.ExecuteAsync(async () =>
{
    var telemetry = source.Next();
    var json = JsonConvert.SerializeObject(telemetry);
    var message = new Message(Encoding.UTF8.GetBytes(json));
    await client.SendEventAsync(message);
},
execOptions);

ConsoleWriter.WriteLine("Close device connection", ConsoleColor.Green);
await client.CloseAsync();

ConsoleWriter.WriteLine($"Stop IoT Device {DEVICE_CONNECTION_STRING.GetDeviceId()}", ConsoleColor.Green);

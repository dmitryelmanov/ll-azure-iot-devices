using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using UniversalDevice;
using UniversalDevice.Options;

const string ENV_PREFIX = "IOT_UD_";

var scenario = Environment.GetEnvironmentVariable($"{ENV_PREFIX}SCENARIO") ?? "Hub";
bool.TryParse(Environment.GetEnvironmentVariable($"{ENV_PREFIX}EDGE"), out var edge);
bool.TryParse(Environment.GetEnvironmentVariable($"{ENV_PREFIX}PNP"), out var pnp);

var buider = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{scenario}.json", optional: true, reloadOnChange: false);

if (edge)
{
    buider.AddJsonFile($"appsettings.{scenario}.Edge.json", optional: true, reloadOnChange: false);
}

if (pnp)
{
    buider.AddJsonFile($"appsettings.PnP.json", optional: true, reloadOnChange: false);
}
   
var config = buider
    .AddEnvironmentVariables(ENV_PREFIX)
    .AddCommandLine(Environment.GetCommandLineArgs())
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

var logger = new SerilogLoggerFactory(Log.Logger)
    .CreateLogger("UniversalDevice");

var deviceOptions = config.GetSection("Device").Get<DeviceOptions>();
var dpsOptions = scenario.Equals("DPS") ? config.GetSection("DPS").Get<DpsOptions>() : null;
var edgeDeviceOptions = edge ? config.GetSection("EdgeDevice").Get<EdgeDeviceOptions>() : null;

logger.LogInformation("Initializing Universal IoT Device");
using var climateDevice = ClimateDeviceFactory.Create(deviceOptions, dpsOptions, edgeDeviceOptions, logger);

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, eventArgs) =>
{
    eventArgs.Cancel = true;
    cts.Cancel();
};
Console.WriteLine("Press Control+C to quit");

await climateDevice.ExecAsync(cts.Token);

logger.LogInformation("Exit Universal IoT Device");

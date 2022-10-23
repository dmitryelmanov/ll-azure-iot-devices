namespace UniversalDevice;

public class TemperatureSource : ITelemetrySource
{
    private readonly Random _random;

    public TemperatureSource(double min, double max)
    {
        _random = new Random();
        Min = min;
        Max = max;
    }

    public double Min { get; set; }
    public double Max { get; set; }

    public Task<Telemetry> NextAsync(CancellationToken cancellationToken = default)
        => Task.FromResult((Telemetry)new TemperatureTelemetry
        {
            Timestamp = DateTimeOffset.Now,
            Value = _random.NextDouble() * (Max - Min) + Min,
        });
}

namespace UniversalDevice;

public sealed class HumidityTelemetry : Telemetry
{
    public override string Type => "humidity";
    public override string Unit => "percent";
}

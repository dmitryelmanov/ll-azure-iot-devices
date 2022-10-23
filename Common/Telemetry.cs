namespace Common;

#nullable disable warnings
public class Telemetry
{
    public DateTimeOffset Timestamp { get; set; }
    public virtual string Type { get; set; }
    public virtual string Unit { get; set; }
    public dynamic Value { get; set; }
}

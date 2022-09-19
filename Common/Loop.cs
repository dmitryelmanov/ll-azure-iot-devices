namespace Common;

public static class Loop
{
    public delegate Task ExecuteMethod();

    public static async Task ExecuteAsync(ExecuteMethod method, Options options)
    {
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };
        Console.WriteLine("Press Control+C to quit");

        try
        {
            while (!cts.IsCancellationRequested)
            {
                await method();
                await Task.Delay(options.IntervalMs, cts.Token);
            }
        }
        catch (OperationCanceledException) { }
    }

    public class Options
    {
        public int IntervalMs { get; set; }
    }
}

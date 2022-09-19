namespace Common;

public static class ConsoleWriter
{
    public static void WriteLine(string? msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
}

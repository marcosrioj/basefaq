using BaseFaq.Tools.Seed.Abstractions;

namespace BaseFaq.Tools.Seed.Infrastructure;

public sealed class ConsoleAdapter : IConsoleAdapter
{
    public void Write(string value)
    {
        Console.Write(value);
    }

    public void WriteLine(string value)
    {
        Console.WriteLine(value);
    }

    public string? ReadLine()
    {
        return Console.ReadLine();
    }
}
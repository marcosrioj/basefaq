using BaseFaq.Faq.Common.Persistence.Seed.Abstractions;

namespace BaseFaq.Faq.Common.Persistence.Seed.Infrastructure;

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
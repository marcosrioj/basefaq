namespace BaseFaq.Faq.Common.Persistence.Seed.Abstractions;

public interface IConsoleAdapter
{
    void Write(string value);
    void WriteLine(string value);
    string? ReadLine();
}
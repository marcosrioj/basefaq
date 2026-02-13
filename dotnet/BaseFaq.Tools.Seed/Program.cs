using BaseFaq.Tools.Seed.Application;

namespace BaseFaq.Tools.Seed;

public static class Program
{
    public static int Main()
    {
        return SeedApplication.Build().Run();
    }
}
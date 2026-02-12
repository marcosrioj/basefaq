using BaseFaq.Faq.Common.Persistence.Seed.Application;

namespace BaseFaq.Faq.Common.Persistence.Seed;

public static class Program
{
    public static int Main()
    {
        return SeedApplication.Build().Run();
    }
}
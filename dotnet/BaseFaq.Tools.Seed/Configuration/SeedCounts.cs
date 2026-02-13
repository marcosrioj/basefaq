namespace BaseFaq.Tools.Seed.Configuration;

public sealed record SeedCounts(
    int UserCount,
    int TenantCount,
    int TenantConnectionsPerApp,
    int FaqCount,
    int ItemsPerFaq,
    int TagCount,
    int ContentRefCount,
    int TagsPerFaq,
    int ContentRefsPerFaq,
    int VotesPerItem)
{
    public static SeedCounts Default => new(
        UserCount: 1,
        TenantCount: 1,
        TenantConnectionsPerApp: 1,
        FaqCount: 24,
        ItemsPerFaq: 12,
        TagCount: 36,
        ContentRefCount: 24,
        TagsPerFaq: 6,
        ContentRefsPerFaq: 4,
        VotesPerItem: 5);
}
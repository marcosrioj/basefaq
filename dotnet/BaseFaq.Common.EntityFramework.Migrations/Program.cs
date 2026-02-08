using BaseFaq.Common.EntityFramework.Migrations.Configuration;
using BaseFaq.Common.EntityFramework.Migrations.Prompts;
using BaseFaq.Common.EntityFramework.Migrations.Runners;
using BaseFaq.Common.EntityFramework.Migrations.Utilities;

namespace BaseFaq.Common.EntityFramework.Migrations;

public static class Program
{
    public static int Main()
    {
        var solutionRoot = SolutionRootLocator.Find();
        var configuration = MigrationsConfiguration.Build(solutionRoot);
        var app = MigrationPrompt.SelectApp();
        var command = MigrationPrompt.SelectCommand();

        string tenantDbConnectionString;
        try
        {
            tenantDbConnectionString = MigrationsConfiguration.GetTenantDbConnectionString(configuration);
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        if (command == MigrationCommand.DatabaseUpdate)
        {
            FaqTenantMigrationUpdater.ApplyAll(configuration, tenantDbConnectionString, app);
            return 0;
        }

        if (solutionRoot is null)
        {
            Console.Error.WriteLine("Unable to locate solution root (BaseFaq.sln).");
            return 1;
        }

        var migrationName = MigrationPrompt.ReadMigrationName();
        return EfMigrationsRunner.AddFaqMigration(solutionRoot, migrationName, app);
    }
}
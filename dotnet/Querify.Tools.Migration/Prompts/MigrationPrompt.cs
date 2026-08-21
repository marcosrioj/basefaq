using Querify.Models.Common.Enums;

namespace Querify.Tools.Migration.Prompts;

internal static class MigrationPrompt
{
    public static ModuleEnum SelectModule()
    {
        Console.WriteLine("Which module?");
        Console.WriteLine("1) QnA");
        Console.WriteLine("2) Direct");
        Console.WriteLine("3) Broadcast");

        while (true)
        {
            Console.Write("Select (1, 2, or 3): ");
            var input = Console.ReadLine();
            if (string.Equals(input, "1", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, ModuleEnum.QnA.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return ModuleEnum.QnA;
            }

            if (string.Equals(input, "2", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, ModuleEnum.Direct.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return ModuleEnum.Direct;
            }

            if (string.Equals(input, "3", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, ModuleEnum.Broadcast.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return ModuleEnum.Broadcast;
            }

            Console.WriteLine("Invalid module.");
        }
    }

    public static MigrationCommand SelectCommand()
    {
        Console.WriteLine("Which command?");
        Console.WriteLine("1) Migrations add");
        Console.WriteLine("2) Database update");

        while (true)
        {
            Console.Write("Select (1 or 2): ");
            var input = Console.ReadLine();
            if (string.Equals(input, "1", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, "migrations add", StringComparison.OrdinalIgnoreCase))
            {
                return MigrationCommand.MigrationsAdd;
            }

            if (string.Equals(input, "2", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(input, "database update", StringComparison.OrdinalIgnoreCase))
            {
                return MigrationCommand.DatabaseUpdate;
            }

            Console.WriteLine("Invalid command.");
        }
    }

    public static string ReadMigrationName()
    {
        while (true)
        {
            Console.Write("Migration name: ");
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("Migration name is required.");
        }
    }
}

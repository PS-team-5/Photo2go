using Microsoft.Data.Sqlite;

namespace Photo2GoAPI.Configuration;

public static class SqliteDatabaseSetup
{
    public static string ResolveConnectionString(IConfiguration configuration, string contentRootPath)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        var connectionStringBuilder = new SqliteConnectionStringBuilder(connectionString);

        if (!Path.IsPathRooted(connectionStringBuilder.DataSource))
        {
            connectionStringBuilder.DataSource = Path.Combine(
                contentRootPath,
                connectionStringBuilder.DataSource);
        }

        return connectionStringBuilder.ToString();
    }

    public static void EnsureLegacyLocationsBaseline(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS "Locations" (
                "id" INTEGER NOT NULL CONSTRAINT "PK_Locations" PRIMARY KEY AUTOINCREMENT,
                "Name" TEXT NOT NULL,
                "ObjectType" TEXT NOT NULL,
                "ArchitectureStyle" TEXT NOT NULL,
                "Period" TEXT NOT NULL,
                "City" TEXT NOT NULL,
                "Province" TEXT NOT NULL,
                "Category" TEXT NOT NULL,
                "BuildingMaterials" TEXT NOT NULL,
                "UnescoStatus" TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}

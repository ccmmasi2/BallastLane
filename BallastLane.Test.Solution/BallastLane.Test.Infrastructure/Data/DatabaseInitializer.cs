using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BallastLane.Test.Infrastructure.Data
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task InitializeAsync()
        {
            await EnsureDatabaseExists();

            await ExecuteScript("Scripts/CreateTables.sql");

            await ExecuteScript("Scripts/SeedData.sql");
        }

        private async Task EnsureDatabaseExists()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            };
            var masterConnectionString = builder.ConnectionString;

            using var connection = new SqlConnection(masterConnectionString);

            await connection.OpenAsync();

            var command = connection.CreateCommand();

            command.CommandText = @"
                                    IF DB_ID('BillingDb') IS NULL
                                    BEGIN
                                        CREATE DATABASE BillingDb
                                    END";

            await command.ExecuteNonQueryAsync();
        }

        private async Task ExecuteScript(string path)
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, path);
            var script   = await File.ReadAllTextAsync(fullPath);

            using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(script, connection);

            await command.ExecuteNonQueryAsync();
        }
    }
}

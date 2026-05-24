using BallastLane.Test.Infrastructure.Authentication;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BallastLane.Test.Infrastructure.Data
{
    public class DatabaseInitializer
    {
        private readonly string         _connectionString;
        private readonly PasswordHasher _passwordHasher;

        public DatabaseInitializer(IConfiguration configuration, PasswordHasher passwordHasher)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _passwordHasher   = passwordHasher;
        }

        public async Task InitializeAsync()
        {
            await EnsureDatabaseExists();
            await EnsureTablesExist();
            await SeedAsync();
        }

        // ── Private ───────────────────────────────────────────────────────────

        private async Task EnsureDatabaseExists()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            };

            using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "IF DB_ID('BillingDb') IS NULL CREATE DATABASE BillingDb";
            await command.ExecuteNonQueryAsync();
        }

        private Task EnsureTablesExist() =>
            ExecuteScriptFileAsync("Scripts/CreateTables.sql");

        private async Task SeedAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await SeedAdminUserAsync(connection);

            // SeedData.sql guards each table with IF NOT EXISTS before inserting.
            await ExecuteScriptFileAsync("Scripts/SeedData.sql");
        }

        private async Task SeedAdminUserAsync(SqlConnection connection)
        {
            using var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Users", connection);
            var count = (int)(await checkCmd.ExecuteScalarAsync())!;
            if (count > 0) return;

            const string sql = @"
                INSERT INTO Users (Username, Email, PasswordHash)
                VALUES (@Username, @Email, @PasswordHash)";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Username",     "CristianMontenegro");
            cmd.Parameters.AddWithValue("@Email",        "CristianMontenegro@test.com");
            cmd.Parameters.AddWithValue("@PasswordHash", _passwordHasher.Hash("password123"));
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task ExecuteScriptFileAsync(string relativePath)
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);
            var script   = await File.ReadAllTextAsync(fullPath);

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(script, connection) { CommandTimeout = 120 };
            await command.ExecuteNonQueryAsync();
        }
    }
}

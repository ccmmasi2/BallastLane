using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Data;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace BallastLane.Test.Infrastructure.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public CategoryRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var categories = new List<Category>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Id, Name, Description FROM Categories ORDER BY Name",
                connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                categories.Add(MapCategory(reader));

            return categories;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Id, Name, Description FROM Categories WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapCategory(reader) : null;
        }

        public async Task<int> CreateAsync(Category category)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                INSERT INTO Categories (Name, Description)
                OUTPUT INSERTED.Id
                VALUES (@Name, @Description)",
                connection);

            command.Parameters.AddWithValue("@Name", category.Name);
            command.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return (int)result!;
        }

        public async Task UpdateAsync(Category category)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                UPDATE Categories
                SET Name = @Name, Description = @Description
                WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id", category.Id);
            command.Parameters.AddWithValue("@Name", category.Name);
            command.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "DELETE FROM Categories WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }

        private static Category MapCategory(SqlDataReader r) => new()
        {
            Id          = r.GetInt32(r.GetOrdinal("Id")),
            Name        = r.GetString(r.GetOrdinal("Name")),
            Description = r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description"))
        };
    }
}

using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Data;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace BallastLane.Test.Infrastructure.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        private const string SelectWithCategory = @"
            SELECT p.Id, p.CategoryId, p.Name, p.Description, p.Price,
                   c.Name AS CategoryName, c.Description AS CategoryDescription
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id";

        public ProductRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = new List<Product>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand($"{SelectWithCategory} ORDER BY p.Name", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                products.Add(MapProduct(reader));

            return products;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand($"{SelectWithCategory} WHERE p.Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapProduct(reader) : null;
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            var products = new List<Product>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                $"{SelectWithCategory} WHERE p.CategoryId = @CategoryId ORDER BY p.Name",
                connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                products.Add(MapProduct(reader));

            return products;
        }

        public async Task<int> CreateAsync(Product product)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                INSERT INTO Products (CategoryId, Name, Description, Price)
                OUTPUT INSERTED.Id
                VALUES (@CategoryId, @Name, @Description, @Price)",
                connection);

            command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);

            var result = await command.ExecuteScalarAsync();
            return (int)result!;
        }

        public async Task UpdateAsync(Product product)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                UPDATE Products
                SET CategoryId = @CategoryId, Name = @Name,
                    Description = @Description, Price = @Price
                WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("DELETE FROM Products WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }

        private static Product MapProduct(SqlDataReader r) => new()
        {
            Id          = r.GetInt32(r.GetOrdinal("Id")),
            CategoryId  = r.GetInt32(r.GetOrdinal("CategoryId")),
            Name        = r.GetString(r.GetOrdinal("Name")),
            Description = r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description")),
            Price       = r.GetDecimal(r.GetOrdinal("Price"))
            //Category    = new Category
            //{
            //    Id          = r.GetInt32(r.GetOrdinal("CategoryId")),
            //    Name        = r.IsDBNull(r.GetOrdinal("CategoryName")) ? string.Empty : r.GetString(r.GetOrdinal("CategoryName")),
            //    Description = r.IsDBNull(r.GetOrdinal("CategoryDescription")) ? null : r.GetString(r.GetOrdinal("CategoryDescription"))
            //}
        };
    }
}

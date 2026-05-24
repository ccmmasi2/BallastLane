using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Data;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace BallastLane.Test.Infrastructure.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public CustomerRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var customers = new List<Customer>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Id, FullName, DocumentNumber, Email, Phone, Address FROM Customers ORDER BY FullName",
                connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                customers.Add(MapCustomer(reader));

            return customers;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Id, FullName, DocumentNumber, Email, Phone, Address FROM Customers WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapCustomer(reader) : null;
        }

        public async Task<int> CreateAsync(Customer customer)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                INSERT INTO Customers (FullName, DocumentNumber, Email, Phone, Address)
                OUTPUT INSERTED.Id
                VALUES (@FullName, @DocumentNumber, @Email, @Phone, @Address)",
                connection);

            command.Parameters.AddWithValue("@FullName", customer.FullName);
            command.Parameters.AddWithValue("@DocumentNumber", (object?)customer.DocumentNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email",          (object?)customer.Email          ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone",          (object?)customer.Phone          ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address",        (object?)customer.Address        ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return (int)result!;
        }

        public async Task UpdateAsync(Customer customer)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                UPDATE Customers
                SET FullName = @FullName, DocumentNumber = @DocumentNumber,
                    Email = @Email, Phone = @Phone, Address = @Address
                WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id",             customer.Id);
            command.Parameters.AddWithValue("@FullName",       customer.FullName);
            command.Parameters.AddWithValue("@DocumentNumber", (object?)customer.DocumentNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email",          (object?)customer.Email          ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone",          (object?)customer.Phone          ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address",        (object?)customer.Address        ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("DELETE FROM Customers WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }

        private static Customer MapCustomer(SqlDataReader r) => new()
        {
            Id             = r.GetInt32(r.GetOrdinal("Id")),
            FullName       = r.GetString(r.GetOrdinal("FullName")),
            DocumentNumber = r.IsDBNull(r.GetOrdinal("DocumentNumber")) ? null : r.GetString(r.GetOrdinal("DocumentNumber")),
            Email          = r.IsDBNull(r.GetOrdinal("Email"))          ? null : r.GetString(r.GetOrdinal("Email")),
            Phone          = r.IsDBNull(r.GetOrdinal("Phone"))          ? null : r.GetString(r.GetOrdinal("Phone")),
            Address        = r.IsDBNull(r.GetOrdinal("Address"))        ? null : r.GetString(r.GetOrdinal("Address"))
        };
    }
}

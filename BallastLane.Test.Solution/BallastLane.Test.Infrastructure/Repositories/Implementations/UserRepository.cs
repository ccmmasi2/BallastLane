using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Data;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace BallastLane.Test.Infrastructure.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public UserRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Id, Username, Email, PasswordHash FROM Users WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapUser(reader) : null;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "SELECT Id, Username, Email, PasswordHash FROM Users WHERE Email = @Email",
                connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapUser(reader) : null;
        }

        public async Task<int> CreateAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                INSERT INTO Users (Username, Email, PasswordHash)
                OUTPUT INSERTED.Id
                VALUES (@Username, @Email, @PasswordHash)",
                connection);

            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            var result = await command.ExecuteScalarAsync();
            return (int)result!;
        }

        private static User MapUser(SqlDataReader r) => new()
        {
            Id           = r.GetInt32(r.GetOrdinal("Id")),
            Username     = r.GetString(r.GetOrdinal("Username")),
            Email        = r.GetString(r.GetOrdinal("Email")),
            PasswordHash = r.GetString(r.GetOrdinal("PasswordHash"))
        };
    }
}

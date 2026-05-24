using BallastLane.Test.Domain.Entities;
using BallastLane.Test.Infrastructure.Data;
using BallastLane.Test.Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace BallastLane.Test.Infrastructure.Repositories.Implementations
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public InvoiceRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            var invoices = new List<Invoice>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                SELECT Id, InvoiceDate, Total, CreatedByUserId,
                       CustomerId, CustomerFullName, CustomerDocumentNumber,
                       CustomerEmail, CustomerPhone, CustomerAddress
                FROM Invoices
                ORDER BY InvoiceDate DESC",
                connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                invoices.Add(MapInvoiceHeader(reader));

            return invoices;
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            Invoice? invoice = null;

            using (var command = new SqlCommand(@"
                SELECT Id, InvoiceDate, Total, CreatedByUserId,
                       CustomerId, CustomerFullName, CustomerDocumentNumber,
                       CustomerEmail, CustomerPhone, CustomerAddress
                FROM Invoices
                WHERE Id = @Id",
                connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    invoice = MapInvoiceHeader(reader);
            }

            if (invoice is null)
                return null;

            using var detailCommand = new SqlCommand(@"
                SELECT Id, InvoiceId, ProductId, ProductName, CategoryName,
                       UnitPrice, Quantity, Subtotal
                FROM InvoiceDetails
                WHERE InvoiceId = @InvoiceId",
                connection);
            detailCommand.Parameters.AddWithValue("@InvoiceId", id);

            using var detailReader = await detailCommand.ExecuteReaderAsync();
            while (await detailReader.ReadAsync())
                invoice.Details.Add(MapDetail(detailReader));

            return invoice;
        }

        public async Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId)
        {
            var invoices = new List<Invoice>();

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                SELECT Id, InvoiceDate, Total, CreatedByUserId,
                       CustomerId, CustomerFullName, CustomerDocumentNumber,
                       CustomerEmail, CustomerPhone, CustomerAddress
                FROM Invoices
                WHERE CustomerId = @CustomerId
                ORDER BY InvoiceDate DESC",
                connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                invoices.Add(MapInvoiceHeader(reader));

            return invoices;
        }

        public async Task<int> CreateAsync(Invoice invoice)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                int invoiceId;

                using (var command = new SqlCommand(@"
                    INSERT INTO Invoices
                        (InvoiceDate, Total, CreatedByUserId,
                         CustomerId, CustomerFullName, CustomerDocumentNumber,
                         CustomerEmail, CustomerPhone, CustomerAddress)
                    OUTPUT INSERTED.Id
                    VALUES
                        (@InvoiceDate, @Total, @CreatedByUserId,
                         @CustomerId, @CustomerFullName, @CustomerDocumentNumber,
                         @CustomerEmail, @CustomerPhone, @CustomerAddress)",
                    connection, transaction))
                {
                    command.Parameters.AddWithValue("@InvoiceDate",              invoice.InvoiceDate);
                    command.Parameters.AddWithValue("@Total",                    invoice.Total);
                    command.Parameters.AddWithValue("@CreatedByUserId",          invoice.CreatedByUserId);
                    command.Parameters.AddWithValue("@CustomerId",               invoice.CustomerId);
                    command.Parameters.AddWithValue("@CustomerFullName",         invoice.CustomerFullName);
                    command.Parameters.AddWithValue("@CustomerDocumentNumber",   (object?)invoice.CustomerDocumentNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerEmail",            (object?)invoice.CustomerEmail          ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerPhone",            (object?)invoice.CustomerPhone          ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerAddress",          (object?)invoice.CustomerAddress        ?? DBNull.Value);

                    var result = await command.ExecuteScalarAsync();
                    invoiceId = (int)result!;
                }

                foreach (var detail in invoice.Details)
                {
                    using var detailCommand = new SqlCommand(@"
                        INSERT INTO InvoiceDetails
                            (InvoiceId, ProductId, ProductName, CategoryName,
                             UnitPrice, Quantity, Subtotal)
                        VALUES
                            (@InvoiceId, @ProductId, @ProductName, @CategoryName,
                             @UnitPrice, @Quantity, @Subtotal)",
                        connection, transaction);

                    detailCommand.Parameters.AddWithValue("@InvoiceId",    invoiceId);
                    detailCommand.Parameters.AddWithValue("@ProductId",    detail.ProductId);
                    detailCommand.Parameters.AddWithValue("@ProductName",  detail.ProductName);
                    detailCommand.Parameters.AddWithValue("@CategoryName", (object?)detail.CategoryName ?? DBNull.Value);
                    detailCommand.Parameters.AddWithValue("@UnitPrice",    detail.UnitPrice);
                    detailCommand.Parameters.AddWithValue("@Quantity",     detail.Quantity);
                    detailCommand.Parameters.AddWithValue("@Subtotal",     detail.Subtotal);

                    await detailCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return invoiceId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using (var deleteDetails = new SqlCommand(
                    "DELETE FROM InvoiceDetails WHERE InvoiceId = @InvoiceId",
                    connection, transaction))
                {
                    deleteDetails.Parameters.AddWithValue("@InvoiceId", id);
                    await deleteDetails.ExecuteNonQueryAsync();
                }

                using (var deleteInvoice = new SqlCommand(
                    "DELETE FROM Invoices WHERE Id = @Id",
                    connection, transaction))
                {
                    deleteInvoice.Parameters.AddWithValue("@Id", id);
                    await deleteInvoice.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static Invoice MapInvoiceHeader(SqlDataReader r) => new()
        {
            Id                     = r.GetInt32(r.GetOrdinal("Id")),
            InvoiceDate            = r.GetDateTime(r.GetOrdinal("InvoiceDate")),
            Total                  = r.GetDecimal(r.GetOrdinal("Total")),
            CreatedByUserId        = r.GetInt32(r.GetOrdinal("CreatedByUserId")),
            CustomerId             = r.GetInt32(r.GetOrdinal("CustomerId")),
            CustomerFullName       = r.GetString(r.GetOrdinal("CustomerFullName")),
            CustomerDocumentNumber = r.IsDBNull(r.GetOrdinal("CustomerDocumentNumber")) ? null : r.GetString(r.GetOrdinal("CustomerDocumentNumber")),
            CustomerEmail          = r.IsDBNull(r.GetOrdinal("CustomerEmail"))          ? null : r.GetString(r.GetOrdinal("CustomerEmail")),
            CustomerPhone          = r.IsDBNull(r.GetOrdinal("CustomerPhone"))          ? null : r.GetString(r.GetOrdinal("CustomerPhone")),
            CustomerAddress        = r.IsDBNull(r.GetOrdinal("CustomerAddress"))        ? null : r.GetString(r.GetOrdinal("CustomerAddress"))
        };

        private static InvoiceDetail MapDetail(SqlDataReader r) => new()
        {
            Id           = r.GetInt32(r.GetOrdinal("Id")),
            InvoiceId    = r.GetInt32(r.GetOrdinal("InvoiceId")),
            ProductId    = r.GetInt32(r.GetOrdinal("ProductId")),
            ProductName  = r.GetString(r.GetOrdinal("ProductName")),
            CategoryName = r.IsDBNull(r.GetOrdinal("CategoryName")) ? null : r.GetString(r.GetOrdinal("CategoryName")),
            UnitPrice    = r.GetDecimal(r.GetOrdinal("UnitPrice")),
            Quantity     = r.GetInt32(r.GetOrdinal("Quantity")),
            Subtotal     = r.GetDecimal(r.GetOrdinal("Subtotal"))
        };
    }
}

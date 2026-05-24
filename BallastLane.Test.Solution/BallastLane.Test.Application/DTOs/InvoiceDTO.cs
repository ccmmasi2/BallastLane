namespace BallastLane.Test.Application.DTOs
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
        public int CreatedByUserId { get; set; }

        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; } = string.Empty;
        public string? CustomerDocumentNumber { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }

        public List<InvoiceDetailDTO> Details { get; set; } = new();
    }
}

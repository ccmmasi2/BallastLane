namespace BallastLane.Test.Domain.Entities
{
    public class Invoice
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

        public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
    }
}

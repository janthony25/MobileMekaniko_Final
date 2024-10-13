namespace MobileMekaniko_Final.Models.Dto
{
    public class AddInvoiceDto
    {
        public int CarId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DueDate { get; set; }
        public required string IssueName { get; set; }
        public string? PaymentTerm { get; set; }
        public string? Notes { get; set; }
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? AmountPaid { get; set; }
        public List<InvoiceItemDto> InvoiceItems { get; set; } = new List<InvoiceItemDto>();
    }
}

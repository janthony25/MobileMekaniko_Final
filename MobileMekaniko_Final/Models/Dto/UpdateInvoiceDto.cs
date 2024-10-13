namespace MobileMekaniko_Final.Models.Dto
{
    public class UpdateInvoiceDto
    {
        public int InvoiceId { get; set; }
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
        public bool IsPaid { get; set; }
        public DateTime DueDate { get; set; }   
        public DateTime DateEdited { get; set; }
    }
}

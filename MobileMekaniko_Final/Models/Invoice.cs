namespace MobileMekaniko_Final.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.Now;
        public DateTime? DateEdited { get; set; }    
        public DateTime? DueDate { get; set; }
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
        public bool? IsEmailSent { get; set; } = false;
        public bool IsPaid { get; set; } = false;

        // FK to Car
        public int CarId { get; set; }
        public Car Car { get; set; }

        // One to Many Invoice-Invoice Item 
        public ICollection<InvoiceItem> InvoiceItem { get; set; }

    }
}

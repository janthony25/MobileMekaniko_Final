namespace MobileMekaniko_Final.Models.Dto
{
    public class InvoiceDetailsDto
    {
        public string CustomerName { get; set; }
        public string? CustomerAddress { get; set; } 
        public string? CustomerEmail { get; set; }
        public string? CustomerNumber { get; set; }  
        public int CarId { get; set; }  
        public string? CarRego { get; set; }
        public string? CarModel { get; set; }
        public string? MakeName { get; set; }
        public int? CarYear { get; set; }
        public int? InvoiceId { get; set; }
        public DateTime? DateAdded { get; set; } 
        public DateTime? DueDate { get; set; }
        public string? IssueName { get; set; }
        public string? PaymentTerm { get; set; }
        public string? Notes { get; set; }
        public decimal? LabourPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? AmountPaid { get; set; }
        public bool? isPaid { get; set; }
        public List<InvoiceItemDto> InvoiceItemDto { get; set; } = new List<InvoiceItemDto>();
    }
}

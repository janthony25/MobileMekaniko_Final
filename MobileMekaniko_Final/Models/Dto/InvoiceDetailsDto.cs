using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class InvoiceDetailsDto
    {
        public string CustomerName { get; set; }
        public string? CustomerAddress { get; set; } 
        public string? CustomerEmail { get; set; }

        [DisplayName("Customer Name")]
        public string? CustomerNumber { get; set; }  
        public int CarId { get; set; }

        [DisplayName("Rego #")]
        public string? CarRego { get; set; }
        public string? CarModel { get; set; }
        public string? MakeName { get; set; }
        public int? CarYear { get; set; }
        public int? InvoiceId { get; set; }

        [DisplayName("Date")]
        public DateTime? DateAdded { get; set; }

        [DisplayName("Due Date")]
        public DateTime? DueDate { get; set; }

        [DisplayName("Issue")]
        public string? IssueName { get; set; }

        [DisplayName("Payment Term")]
        public string? PaymentTerm { get; set; }
        public string? Notes { get; set; }

        [DisplayName("Labor Price")]
        public decimal? LabourPrice { get; set; }
        public decimal? Discount { get; set; }

        [DisplayName("Shipping/Travel Fee")]
        public decimal? ShippingFee { get; set; }

        [DisplayName("Sub Total")]
        public decimal? SubTotal { get; set; }
        [DisplayName("GST")]
        public decimal? TaxAmount { get; set; }

        [DisplayName("Total Amount")]
        public decimal? TotalAmount { get; set; }

        [DisplayName("Amount Paid")]
        public decimal? AmountPaid { get; set; }

        [DisplayName("Status")]
        public bool? isPaid { get; set; }
        public List<InvoiceItemDto> InvoiceItemDto { get; set; } = new List<InvoiceItemDto>();
    }
}

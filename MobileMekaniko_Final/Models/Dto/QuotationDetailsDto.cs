using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class QuotationDetailsDto
    {
        public int? CustomerId { get; set; } 
        [DisplayName("Customer Name")]
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerNumber { get; set; }
        public int? CarId { get; set; }

        [DisplayName("Rego #")]
        public string? CarRego { get; set; }
        public string? CarModel { get; set; }
        public string? MakeName { get; set; }
        public int? CarYear { get; set; }
        public int QuotationId { get; set; }

        [DisplayName("Date")]
        public DateTime DateAdded { get; set; } = DateTime.Now;

        [DisplayName("Date Edited")]
        public DateTime? DateEdited { get; set; }

        [DisplayName("Issue")]
        public string? IssueName { get; set; }
        public string? Notes { get; set; }

        [DisplayName("Labor Price")]
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }

        [DisplayName("Shipping/Travel Fee")]
        public decimal? ShippingFee { get; set; }

        [DisplayName("Sub Total")]
        public decimal? SubTotal { get; set; }

        [DisplayName("GST")]
        public decimal? TaxAmount { get; set; }

        [DisplayName("Total Amount")]
        public decimal? TotalAmount { get; set; }
        public bool? IsEmailSent { get; set; } // will mark the email as sent
        public List<QuotationItemDto> QuotationItemDto { get; set; } = new List<QuotationItemDto>();
    }
}

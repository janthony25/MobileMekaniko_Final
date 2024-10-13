using System.ComponentModel.DataAnnotations;

namespace MobileMekaniko_Final.Models
{
    public class Quotation
    {
        [Key]
        public int QuotationId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime? DateEdited { get; set; }   
        public required string IssueName { get; set; }
        public string? Notes { get; set; }
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TaxAmount { get; set; }     
        public decimal? TotalAmount { get; set; }
        public bool? IsEmailSent { get; set; } // will mark the email as sent

        // One to Many Quotation-Quotation Item
        public ICollection<QuotationItem> QuotationItem { get; set; }  

        // FK to Car
        public int CarId { get; set; }
        public Car Car { get; set; }
    }
}

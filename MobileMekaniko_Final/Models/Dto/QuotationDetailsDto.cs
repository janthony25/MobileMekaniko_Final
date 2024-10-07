namespace MobileMekaniko_Final.Models.Dto
{
    public class QuotationDetailsDto
    {
        public int? CustomerId { get; set; } 
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerNumber { get; set; }
        public int? CarId { get; set; }
        public string? CarRego { get; set; }
        public string? CarModel { get; set; }
        public string? MakeName { get; set; }
        public int? CarYear { get; set; }
        public int QuotationId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime? DateEdited { get; set; }
        public string? IssueName { get; set; }
        public string? Notes { get; set; }
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool? IsEmailSent { get; set; } // will mark the email as sent
        public List<QuotationItemDto> QuotationItemDto { get; set; } = new List<QuotationItemDto>();
    }
}

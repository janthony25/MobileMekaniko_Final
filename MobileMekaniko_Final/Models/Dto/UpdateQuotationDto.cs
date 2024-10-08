namespace MobileMekaniko_Final.Models.Dto
{
    public class UpdateQuotationDto
    {
        public int QuotationId { get; set; }
        public string IssueName { get; set; }
        public string? Notes { get; set; }
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool DateEdited { get; set; }    
    }
}

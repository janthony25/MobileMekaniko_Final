namespace MobileMekaniko_Final.Models.Dto
{
    public class AddQuotationDto
    {
        public int CarId { get; set; }
        public required string IssueName { get; set; }
        public string? Notes { get; set; }
        public decimal? LaborPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public List<QuotationItemDto> QuotationItems { get; set; } = new List<QuotationItemDto>();
    }
}

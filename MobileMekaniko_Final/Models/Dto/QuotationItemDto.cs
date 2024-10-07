namespace MobileMekaniko_Final.Models.Dto
{
    public class QuotationItemDto
    {
        public int QuotationItemId { get; set; }
        public required string ItemName { get; set; }
        public required int Quantity { get; set; }
        public required decimal ItemPrice { get; set; }
        public decimal? ItemTotal { get; set; }
    }
}

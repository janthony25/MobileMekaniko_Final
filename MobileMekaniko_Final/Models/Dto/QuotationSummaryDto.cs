namespace MobileMekaniko_Final.Models.Dto
{
    public class QuotationSummaryDto
    {
        public int QuotationId { get; set; }
        public string IssueName { get; set; }
        public DateTime DateAdded { get; set; }
        public decimal? TotalAmount { get; set; }    
    }
}

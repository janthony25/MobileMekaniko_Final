namespace MobileMekaniko_Final.Models.Dto
{
    public class InvoiceListDto
    {
        public string CustomerName { get; set; }
        public string CarRego { get; set; }
        public int InvoiceId { get; set; }
        public string IssueName { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? AmountPaid { get; set; }
        public bool? IsPaid { get; set; }    

    }
}

namespace MobileMekaniko_Final.Models
{
    public class InvoiceItem
    {
        public int InvoiceItemId { get; set; }
        public required string ItemName { get; set; }
        public required int Quantity { get; set; }
        public required decimal ItemPrice { get; set; }
        public decimal? ItemTotal { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime? DateEdited { get; set; }

        // FK to Invoice
        public int InvoiceId { get; set; }  
        public Invoice Invoice { get; set; }
    }
}

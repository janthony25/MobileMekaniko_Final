using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class InvoiceSummaryDto
    {
        [DisplayName("Invoice ID")]
        public int InvoiceId { get; set; }

        [DisplayName("Issue")]
        public string? IssueName { get; set; }

        [DisplayName("Date Issued")]
        public DateTime? DateAdded { get; set; }

        [DisplayName("Due Date")]
        public DateTime? DueDate { get; set; }

        [DisplayName("Total Amount")]
        public decimal? TotalAmount { get; set; }

        [DisplayName("Amount Paid")]
        public decimal? AmountPaid { get; set; }

        [DisplayName("Status")]
        public bool? isPaid { get; set; }
    }
}

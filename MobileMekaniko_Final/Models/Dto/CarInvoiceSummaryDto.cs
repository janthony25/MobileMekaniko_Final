using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class CarInvoiceSummaryDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerNumber { get; set; }
        public int CarId { get; set; }
        public string CarRego { get; set; }
        public string MakeName { get; set; }
        public string? CarModel { get; set; }
        public int? CarYear { get; set; }

        public List<InvoiceSummaryDto> Invoices { get; set; }
    }
}

using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class CustomerListSummaryDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerNumber { get; set; }
    }
}

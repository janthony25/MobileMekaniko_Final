using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class CustomerListSummaryDto
    {
        [DisplayName("ID")]
        public int CustomerId { get; set; }

        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }

        [DisplayName("Email Address")]
        public string? CustomerEmail { get; set; }

        [DisplayName("Contact #")]
        public string? CustomerNumber { get; set; }
    }
}

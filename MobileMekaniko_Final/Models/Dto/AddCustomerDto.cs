using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class AddCustomerDto
    {
        [DisplayName("Customer Name")]
        public required string CustomerName { get; set; }

        [DisplayName("Address")]
        public string? CustomerAddress { get; set; }

        [DisplayName("Email Address")]
        public string? CustomerEmail { get; set; }

        [DisplayName("Contact #")]
        public string? CustomerNumber { get; set; }
    }
}

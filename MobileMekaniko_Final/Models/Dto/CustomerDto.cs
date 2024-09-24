using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class CustomerDto
    {
        [DisplayName("Customer ID")]
        public int CustomerId { get; set; }

        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }

        [DisplayName("Address")]
        public string? CustomerAddress { get; set; }

        [DisplayName("Email Address")]
        public string? CustomerEmail { get; set; }

        [DisplayName("Contact #")]
        public string? CustomerNumber { get; set; }
        public List<CarDto> CarDto { get; set; }       
    }
}

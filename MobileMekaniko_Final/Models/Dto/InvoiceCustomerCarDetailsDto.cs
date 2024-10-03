using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class InvoiceCustomerCarDetailsDto
    {
        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }

        [DisplayName("Rego #")]
        public string CarRego { get; set; }
        public int CarId { get; set; }

    }
}

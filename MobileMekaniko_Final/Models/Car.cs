using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MobileMekaniko_Final.Models
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        [DisplayName("Rego #")]
        public required string CarRego { get; set; }

        [DisplayName("Car Model")]
        public string? CarModel { get; set; }    

        [DisplayName("Car Year")]
        public int? CarYear { get; set; }

        [DisplayName("Date Added")]
        public DateTime CarAdded { get; set; } = DateTime.Now;

        [DisplayName("DateEdited")]
        public DateTime? DateEdited { get; set; }

        // Many to Many Car-Make
        public List<CarMake> CarMake { get; set; } 
            

        // FK to Customer
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }  
    }
}

using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class CarDetailsDto
    {
        // Customer ID
        public int CustomerId { get; set; }

        // Car Details
        [DisplayName("Car ID")]
        public int CarId { get; set; }

        [DisplayName("Rego #")]
        public required string CarRego { get; set; }

        [DisplayName("Car Model")]
        public string? CarModel { get; set; }

        [DisplayName("Car Year")]
        public int? CarYear { get; set; }

        [DisplayName("Date Added")]
        public DateTime? DateAdded { get; set; }

        [DisplayName("DateEdited")]
        public DateTime? DateEdited { get; set; }

        // Make Details
        public int MakeId { get; set; }
        public string MakeName { get; set; }    

    }
}

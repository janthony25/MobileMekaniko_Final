using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class CarDto
    {
        [DisplayName("ID")]
        public int CarId { get; set; }

        [DisplayName("Rego #")]
        public string CarRego { get; set; }

        [DisplayName("Model")]
        public string CarModel { get; set; }

        [DisplayName("Year")]
        public int? CarYear { get; set; }

        public List<MakeDto> MakeDto { get; set; }  
    }
}

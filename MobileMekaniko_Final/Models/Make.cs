using System.ComponentModel;

namespace MobileMekaniko_Final.Models
{
    public class Make
    {
        public int MakeId { get; set; }

        [DisplayName("Make")]
        public required string MakeName { get; set; }

        // Many to Many Car-Make
        public List<CarMake> CarMake { get; set; }  
    }
}

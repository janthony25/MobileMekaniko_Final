using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class AddMakeDto
    {
        [DisplayName("Make")]
        public required string MakeName { get; set; }   
    }
}

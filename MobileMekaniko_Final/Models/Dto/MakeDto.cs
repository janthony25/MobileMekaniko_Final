using System.ComponentModel;

namespace MobileMekaniko_Final.Models.Dto
{
    public class MakeDto
    {
        public int MakeId { get; set; }

        [DisplayName("Make")]
        public string MakeName { get; set; }    
    }
}

using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Repository.IRepository
{
    public interface ICarRepository
    {
        Task<List<MakeDto>> GetMakesAsync();
    }
}

using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Repository.IRepository
{
    public interface IMakeRepository
    {
        Task AddMakeAsync(AddMakeDto dto);
        Task<List<MakeDetailsDto>> GetMakeListAsync();
        Task DeleteMakeAsync(int id);
    }
}

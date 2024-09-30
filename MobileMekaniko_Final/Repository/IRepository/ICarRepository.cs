using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Repository.IRepository
{
    public interface ICarRepository
    {
        Task<List<MakeDto>> GetMakesAsync();
        Task<CarDetailsDto> GetCarDetailsAsync(int id);
        Task AddCarAsync(CarDetailsDto dto);
        Task UpdateCarAsync(CarDetailsDto dto);
        Task DeleteCarAsync(int id);
     }
}

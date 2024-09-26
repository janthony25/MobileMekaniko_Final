using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Repository
{
    public class CarRepository : ICarRepository
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger<CarRepository> _logger;

        public CarRepository(ApplicationDbContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<CarRepository>();
        }
        public async Task<List<MakeDto>> GetMakesAsync()
        {
            try
            {
                // Fetching Car Makes
                var makes = await _data.Makes
                    .Select(m => new MakeDto
                    {
                        MakeId = m.MakeId,
                        MakeName = m.MakeName
                    }).ToListAsync();

                _logger.LogInformation($"Successfully fetched {makes.Count} Car Makes.");
                return makes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching car makes.");
                throw;
            }
           
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Models;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;
using SQLitePCL;

namespace MobileMekaniko_Final.Repository
{
    public class MakeRepository : IMakeRepository
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger<MakeRepository> _logger;

        public MakeRepository(ApplicationDbContext data, ILoggerFactory logger)
        {
            _data = data;
            _logger = logger.CreateLogger<MakeRepository>();
        }
        public async Task AddMakeAsync(AddMakeDto dto)
        {
            try
            {
                // Check if the make already exists
                var existingMake = await _data.Makes
                    .FirstOrDefaultAsync(m => m.MakeName.ToLower() == dto.MakeName.ToLower());

                if (existingMake != null)
                {
                    _logger.LogWarning($"Make {dto.MakeName} already exists in the database");
                    throw new InvalidOperationException("Car make already exists");
                }

                // Proceed with adding the new make if it doesn't exist
                var make = new Make
                {
                    MakeName = dto.MakeName
                };

                _data.Makes.Add(make);
                _logger.LogInformation($"Successfully added {dto.MakeName} to the database.");
                await _data.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding {dto.MakeName} to the database");
                throw;
            }
        }

        public async Task DeleteMakeAsync(int id)
        {
            try
            {
                // Find make by id
                var make = await _data.Makes.FindAsync(id);

                if (make == null || make.MakeId == 0)
                {
                    _logger.LogWarning("Car make not found.");
                    throw new KeyNotFoundException("Car make not found.");
                }

                _data.Makes.Remove(make);
                _logger.LogInformation($"Successfully removed {make.MakeName} from the database.");
                await _data.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred whilt trying to delete car make.");
                throw;
            }
        }

        public async Task<List<MakeDetailsDto>> GetMakeListAsync()
        {
            try
            {
                var makes = await _data.Makes
                    .Select(m => new MakeDetailsDto
                    {
                        MakeId = m.MakeId,
                        MakeName = m.MakeName
                    }).ToListAsync();

                _logger.LogInformation($"Fetched {makes.Count} car makes.");
                return makes;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching make list.");
                throw;
            }
        }
    }
}

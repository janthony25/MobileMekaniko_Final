
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Models;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;
using System.Linq.Expressions;

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

        public async Task AddCarAsync(CarDetailsDto dto)
        {
            try
            {
                // Get the customer by Id
                var customer = await _data.Customers.FindAsync(dto.CustomerId);

                // Adding new car
                var newCar = new Car
                {
                    CarRego = dto.CarRego,
                    CarModel = dto.CarModel,
                    CarYear = dto.CarYear,
                    CustomerId = dto.CustomerId
                };

                // Adding car to db
                _data.Cars.Add(newCar);
                _logger.LogInformation($"Successfully added car with Rego {newCar.CarRego}");

                await _data.SaveChangesAsync();

                // find make using id
                var make = await _data.Makes.FindAsync(dto.MakeId);

                // Creating new Car Make
                var carMake = new CarMake
                {
                    CarId = newCar.CarId,
                    MakeId = make.MakeId
                };

                // Adding new CarMake to db
                _data.CarMakes.Add(carMake);
                _logger.LogInformation($"Successfully added new CarMake with carId {carMake.CarId} and makeId {carMake.MakeId}");

                await _data.SaveChangesAsync();
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to add new car");
                throw;
            }
        }

        public async Task<CarDetailsDto> GetCarDetailsAsync(int id)
        {
            try
            {
                // Trying to fetch car details
                var car = await _data.Cars
                    .Where(car => car.CarId == id)
                    .Select(car => new CarDetailsDto
                    {
                        CarId = car.CarId,
                        CarRego = car.CarRego,
                        MakeName = car.CarMake.Select(cm => cm.Make.MakeName).FirstOrDefault(),
                        CarModel = car.CarModel,
                        CarYear = car.CarYear,
                        DateAdded = car.DateAdded,
                        DateEdited = car.DateEdited,
                        CustomerId = car.CustomerId
                    }).FirstOrDefaultAsync();

                if (car == null || car.CarId == null)
                {
                    _logger.LogInformation("No car details was found.");
                    return car;
                }

                _logger.LogInformation($"Successfully fetched car details of car with id {id}");
                return car;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to fetch car details of car with id {id}");
                throw;
            }
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

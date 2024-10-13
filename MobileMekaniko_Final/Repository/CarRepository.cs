
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Data.Migrations;
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

        public async Task DeleteCarAsync(int id)
        {
            try
            {
                // Find car by id
                var car = await _data.Cars.FindAsync(id);

                if(car == null || car.CarId == 0)
                {
                    _logger.LogWarning($"Car with id {id} not found.");
                    throw new KeyNotFoundException("Car not found.");
                }

                _data.Cars.Remove(car);
                _logger.LogInformation($"Deleting car with id {id}.");
                await _data.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting car.");
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
                        MakeId = car.CarMake.FirstOrDefault().MakeId, // Get the associated MakeId directly
                        MakeName = car.CarMake.FirstOrDefault().Make.MakeName, // Get the associated MakeName directly
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

        public async Task<CarInvoiceSummaryDto> GetCarInvoiceSummaryAsync(int id)
        {
            try
            {
                // Find car using Id
                var car = await _data.Cars
                    .Include(car => car.Customer)
                    .Include(car => car.CarMake)
                        .ThenInclude(cm => cm.Make)
                    .Include(car => car.Invoice)
                    .Where(car => car.CarId == id)
                    .Select(car => new CarInvoiceSummaryDto
                    {
                        CustomerId = car.Customer.CustomerId,
                        CustomerName = car.Customer.CustomerName,
                        CustomerEmail = car.Customer.CustomerEmail,
                        CustomerNumber = car.Customer.CustomerNumber,
                        CarId = car.CarId,
                        CarRego = car.CarRego,
                        MakeName = car.CarMake.FirstOrDefault().Make.MakeName,
                        CarModel = car.CarModel,
                        CarYear = car.CarYear,
                        Invoices = car.Invoice.Select(i => new InvoiceSummaryDto
                        {
                            InvoiceId = i.InvoiceId,
                            IssueName = i.IssueName,
                            DateAdded = i.DateAdded,
                            DueDate = i.DueDate,
                            TotalAmount = i.TotalAmount,
                            AmountPaid = i.AmountPaid,
                            isPaid = i.IsPaid
                        }).ToList()
                    }).FirstOrDefaultAsync();

                if(car == null || car.CarId == 0)
                {
                    _logger.LogWarning("Invalid car id");
                    throw new KeyNotFoundException("Invalid car id");
                }

                _logger.LogInformation($"Successfully fetched invoice details for car with Id {id}");
                return car;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"An error occurred while fetching car invoice summary.");
                throw;  
            }
        }

        public async Task<List<MakeDto>> GetMakesAsync()
        {
            try
            {
                // Fetching Car Makes
                var makes = await _data.Makes
                    .OrderBy(m => m.MakeName)
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

        public async Task UpdateCarAsync(CarDetailsDto dto)
        {
            try
            {
                // Find car using id
                var car = await _data.Cars.FindAsync(dto.CarId);

                if (car == null || car.CarId == 0)
                {
                    _logger.LogWarning($"No car was found with id {dto.CarId}");
                    throw new KeyNotFoundException("No car was found.");
                }

                car.CarRego = dto.CarRego;
                car.CarModel = dto.CarModel;
                car.CarYear = dto.CarYear;
                car.DateEdited = DateTime.Now;

                // Save changes to db
                await _data.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated car.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating car details.");
                throw;
            }
        }
    }
}

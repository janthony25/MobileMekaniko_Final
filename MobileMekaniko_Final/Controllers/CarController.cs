using Microsoft.AspNetCore.Mvc;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Controllers
{
    public class CarController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CarRepository> _logger;

        public CarController(IUnitOfWork unitOfWork, ILogger<CarRepository> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;   
        }
        
        // GET : Get Car Details
        public async Task<IActionResult> GetCarDetails(int id)
        {
            try
            {
                _logger.LogInformation($"Request to fetch car details for Car Id {id}");

                var car = await _unitOfWork.Car.GetCarDetailsAsync(id);
                var makes = await _unitOfWork.Car.GetMakesAsync();
                ViewBag.Makes = makes;

                _logger.LogInformation($"Successfully fetched car details. returning car details");
                return Json(new { car, makes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching car details");
                return Json(new { success = false, message = "An error occurred while fetching car details." });
            }
        }

        // POST: Add Car
        public async Task<IActionResult> AddCar(CarDetailsDto dto)
        {
            try
            {
                _logger.LogInformation("Request to add new car.");
                if (ModelState.IsValid)
                {
                    await _unitOfWork.Car.AddCarAsync(dto);
                    _logger.LogInformation("Car added successfully.");
                    return Json(new { success = true, message = "Car added successfully." });
                }
                _logger.LogWarning("Invalid model state");

                return Json(new { success = false, message = "Invalid data provided." });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding car to customer.");
                return Json(new { success = false, message = "An error occurred while adding new car to customer." });
            }
        }
    }
}

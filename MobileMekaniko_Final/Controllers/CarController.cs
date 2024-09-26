using Microsoft.AspNetCore.Mvc;
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
    }
}

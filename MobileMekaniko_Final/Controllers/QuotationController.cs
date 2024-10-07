using Microsoft.AspNetCore.Mvc;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Controllers
{
    public class QuotationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuotationController> _logger;

        public QuotationController(IUnitOfWork unitOfWork, ILogger<QuotationController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET : Car Quotation
        public async Task<IActionResult> GetCarQuotation(int id)
        {
            try
            {
                _logger.LogInformation($"Request to fetch car details of car with id {id}");
                var car = await _unitOfWork.Quotation.GetCarQuotationSummaryAsync(id);

                if(car == null)
                {
                    _logger.LogWarning($"No details was found for car with id {id}");
                    return NotFound();
                }

                return View(car);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching car details for car with id {id}");
                return StatusCode(500, "An error occurred while fetching car details");
            }
        }

        // POST : Add Quotation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuotation([FromBody] AddQuotationDto dto)
        {
            try
            {
                _logger.LogInformation($"Received DTO: {System.Text.Json.JsonSerializer.Serialize(dto)}");
                _logger.LogInformation($"Request to add quotation to car with id {dto.CarId}");

                if (ModelState.IsValid)
                {
                    await _unitOfWork.Quotation.AddQuotationAsync(dto);
                    _logger.LogInformation($"Successfully added quotation to car with id {dto.CarId}");
                    return Json(new { success = true, message = "Successfully added new quotation" });
                }

                _logger.LogWarning($"Invalid data provided.");
                return Json(new {success=false,message="Invalid data provided."});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to add quotation to car with id {dto.CarId}");
                return Json(new { success = false, message = "An error occurred while trying to add new quotation" });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
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
    }
}

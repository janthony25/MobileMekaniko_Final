using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvoiceController> _logger;    

        public InvoiceController(IUnitOfWork unitOfWork, ILogger<InvoiceController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        

        // GET : invoice details
        public async Task<IActionResult> GetInvoiceDetails(int id)
        {
            try
            {
                _logger.LogInformation($"Request to get invoice details of invoice with id {id}");
                var invoice = await _unitOfWork.Invoice.GetInvoiceDetailsAsync(id);

                _logger.LogInformation($"Successfully fetched invoice details of invoice with id {id}");
                return Json(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching invoice details");
                return Json(new { success = false, message = "An error occurred while fetching invoice details" });
            }
        }

        // GET : Customer Car details by car id
        public async Task<IActionResult> GetCustomerCarDetails(int id)
        {
            try
            {
                _logger.LogInformation($"Request to fetched customer and car details for car id {id}");
                var customerCar = await _unitOfWork.Invoice.GetCustomerCarDetailsAsync(id);

                _logger.LogInformation($"Successfully fetched customer and car details for car with id {id} ");
                return Json(customerCar);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while trying to fetch customer and car details for car id {id}");
                return Json(new { success = false, message = "An error occurred while trying to fetch customer and car details." });
            }
        }

        // POST : Add Invoice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInvoice([FromBody]AddInvoiceDto dto)
        {
            try
            {
                _logger.LogInformation("Request to add new invoice");
                _logger.LogInformation($"Received DTO: {System.Text.Json.JsonSerializer.Serialize(dto)}");
                if (ModelState.IsValid)
                {
                    await _unitOfWork.Invoice.AddInvoiceAsync(dto);
                    _logger.LogInformation($"Successfully added new invoice to car with id {dto.CarId}");
                    return Json(new { success = true, message = "Successfully added new invoice!" });
                }

                _logger.LogWarning("Invalid data provided");
                return Json(new { success = false, message = "Invalid data provided" });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding new invoice.");
                return Json(new { success = false, message = "An error occurred while adding new Invoice." });
            }
        }
    }
}

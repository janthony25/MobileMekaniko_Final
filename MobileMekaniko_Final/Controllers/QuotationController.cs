
using Microsoft.AspNetCore.Mvc;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;
using MobileMekaniko_Final.Services;
using PdfSharp.Quality;
using System.Text.RegularExpressions;

namespace MobileMekaniko_Final.Controllers
{
    public class QuotationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuotationController> _logger;
        private readonly IQuotationPdfService _quotationPdfService;
        private readonly EmailPdfService _emailPdfService;

        public QuotationController(
                IUnitOfWork unitOfWork,
                ILogger<QuotationController> logger,
                IQuotationPdfService quotationPdfService,
                EmailPdfService emailPdfService)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
                _quotationPdfService = quotationPdfService;
                _emailPdfService = emailPdfService;
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

        // GET : Quotation Details
        public async Task<IActionResult> GetQuotationDetails(int id)
        {
            try
            {
                _logger.LogInformation($"Getting details for quotation with id {id}");
                var quotation = await _unitOfWork.Quotation.GetQuotationDetailsAsync(id);

                if (quotation == null)
                {
                    _logger.LogWarning($"No data found for quotation with id {id}");
                    return Json(new { success = false, message = "No data found" });
                }

                _logger.LogInformation($"Successfully fetched quotation details for quotation with id {id}");
                return Json(quotation);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching details for quotation with id {id}");
                return Json(new { success = false, message = "An error occurred while fetching quotation details" });
            }
        }

        // POST : Update Quotation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuotation([FromBody] UpdateQuotationDto dto)
        {
            try
            {
                _logger.LogInformation($"Request to update quotation with id {dto.QuotationId}");

                if (ModelState.IsValid)
                {
                    await _unitOfWork.Quotation.UpdateQuotationAsync(dto);
                    _logger.LogInformation($"Successfully updated quotation with id {dto.QuotationId}");
                    return Json(new { success = true, message = "Quotation successfully edited." });
                }

                _logger.LogWarning($"Invalid modelstate");
                return Json(new { success = false, message = "Invalid data provided" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating quotation with id {dto.QuotationId}");
                return Json(new { success = false, message="An error occurred while updating quotation" });
            }
        }

        // POST : Delete Quotation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuotation(int id)
        {
            try
            {
                _logger.LogInformation($"Request to delete quotation with id {id}");

                await _unitOfWork.Quotation.DeleteQuotationAsync(id);

                _logger.LogInformation($"Successfully deleted quotation with id {id}");
                return Json(new { success = true, message = "Quote successfully deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to delete quote with id {id}");
                return Json(new { success = false, message = "An error occurred while trying to delete quote." });
            }
        }

        // GET : Convert Quotation to PDF
        public async Task<IActionResult> ViewPdf(int id)
        {
            try
            {
                var quotation = await _unitOfWork.Quotation.GetQuotationDetailsAsync(id);

                if(quotation == null)
                {
                    return NotFound();
                }

                var pdfBytes = _quotationPdfService.CreateQuotationPdf(quotation);
                return File(pdfBytes, "application/pdf");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while generating PDF for quotation with id {id}");
                return StatusCode(500, "An error occurred while generating the PDF");
            }
        }

        // GET : Download Quotation PDF
        public async Task<IActionResult> DownloadPdf(int id)
        {
            _logger.LogInformation($"Request to download PDF for quotation with id {id}");
            try
            {
                var quotation = await _unitOfWork.Quotation.GetQuotationDetailsAsync(id);
                if (quotation == null)
                {
                    _logger.LogWarning($"Quotation not fond for ID {id}");
                    return NotFound();
                }

                _logger.LogInformation($"Generating PDF for quotation ID {id}");
                var pdfBytes = _quotationPdfService.CreateQuotationPdf(quotation);

                string safeCarRego = Regex.Replace(quotation.CarRego, @"[^\w\-]", "_");
                string fileName = $"{safeCarRego}_Quotation_{id}.pdf";

                _logger.LogInformation($"Returning PDF file: {fileName}");
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while generating PDF for quotation {id}");
                return StatusCode(500, "An error occurred while generating the PDF.");
            }
        }

        // POST : Send Quotation PDF to Email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendQuotationEmail(int quotationId)
        {
            try
            {
                _logger.LogInformation($"Request to send quotation email for quotation with id {quotationId}");

                if (quotationId <= 0)
                {
                    _logger.LogWarning("Invalid quotation ID received.");
                    return Json(new { success = false, message = "Invalid quotation ID." });
                }

                var quotation = await _unitOfWork.Quotation.GetQuotationDetailsAsync(quotationId);
                if (quotation == null)
                {
                    _logger.LogWarning($"Quotation not found with id {quotationId}");
                    return Json(new { success = false, message = "Quotation not found." });
                }

                // Create the PDF from quotation details
                var pdfBytes = _quotationPdfService.CreateQuotationPdf(quotation);

                // Prepare email details
                var subject = $"Quotation #{quotation.QuotationId} from Mobile Mekaniko";
                var body = $"Dear {quotation.CustomerName},\n\nPlease find attached your quotation #{quotation.QuotationId}.\n\nThank you,\nMobile Mekaniko";
                var email = quotation.CustomerEmail;

                // Use EmailPdfService to send the quotation email with the PDF attachment
                await _emailPdfService.SendInvoiceEmailAsync(email, subject, body, pdfBytes, $"Quotation_{quotation.QuotationId}.pdf");

                await _unitOfWork.Quotation.UpdateIsEmailSendAsync(quotationId, true);
                
                _logger.LogInformation($"Quotation email successfully sent to {email} for quotation with id {quotationId}");
                return Json(new { success = true, message = "Quotation email successfully sent." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while sending quotation email for quotation with id {quotationId}");
                return Json(new { success = false, message = "An error occurred while sending the quotation email." });
            }
        }

        // GET : Quoataion List
        public async Task<IActionResult> GetQuotationList()
        {
            try
            {
                _logger.LogInformation($"Request to fetch quotation list.");

                var quotations = await _unitOfWork.Quotation.GetQuotationListAsync();

                _logger.LogInformation($"quotation list fetched successfully.");
                return View(quotations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching quotation list.");
                return StatusCode(500, "An error occurred while trying to fetch quotaion list.");
            }
        }
    }
}

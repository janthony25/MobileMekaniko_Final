using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using MobileMekaniko_Final.Models;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;
using MobileMekaniko_Final.Services;
using NuGet.Protocol.Plugins;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace MobileMekaniko_Final.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvoiceController> _logger;    
        private readonly IInvoicePdfService _invoicePdfService;
        private readonly EmailPdfService _emailPdfService;

        public InvoiceController(IUnitOfWork unitOfWork, ILogger<InvoiceController> logger, IInvoicePdfService invoicePdfService, EmailPdfService emailPdfService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _invoicePdfService = invoicePdfService;
            _emailPdfService = emailPdfService;
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

        // POST : Update Invoice
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInvoice([FromBody] UpdateInvoiceDto dto)
        {
            try
            {
                _logger.LogInformation($"Request to update invoice with id {dto.InvoiceId}");
                if (ModelState.IsValid)
                {
                    await _unitOfWork.Invoice.UpdateInvoiceAsync(dto);
                    _logger.LogInformation($"Successfully updated invoice with id {dto.InvoiceId}");
                    return Json(new { success = true, message = "Invoice successfully updated." });
                }

                _logger.LogWarning($"Invalid data");
                return Json(new { success = false, message = "Invalid data provided" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating invoice with id {dto.InvoiceId}");
                return Json(new { success = false, message = "An error occurred while updating invoice" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                _logger.LogInformation($"Request to delete invoice with id {id}");
                await _unitOfWork.Invoice.DeleteInvoiceAsync(id);

                if(id == 0)
                {
                    _logger.LogWarning($"No invoice found with id of {id}");
                    return Json(new { success = false, message = "Invoice not found." });
                }

                return Json(new { success = true, message = "Invoice successfully deleted." });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting invoice with id {id}");
                return Json(new { success = false, message = "An error occurred while deleting invoice" });
            }
        }

        // GET : Convert Invoice to PDF
        [HttpGet]
        public async Task<IActionResult> ViewPdf(int id)
        {
            try
            {
                var invoice = await _unitOfWork.Invoice.GetInvoiceDetailsAsync(id);
                if (invoice == null)
                {
                    return NotFound();
                }

                var pdfBytes = _invoicePdfService.CreateInvoicePdf(invoice);
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while generating PDF for invoice {id}");
                return StatusCode(500, "An error occurred while generating the PDF.");
            }
        }

        // GET : Download Invoice PDF
        [HttpGet]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            _logger.LogInformation($"DownloadPdf action called for invoice ID: {id}");
            try
            {
                var invoice = await _unitOfWork.Invoice.GetInvoiceDetailsAsync(id);
                if (invoice == null)
                {
                    _logger.LogWarning($"Invoice not found for ID: {id}");
                    return NotFound();
                }

                _logger.LogInformation($"Generating PDF for invoice ID: {id}");
                var pdfBytes = _invoicePdfService.CreateInvoicePdf(invoice);

                string safeCarRego = Regex.Replace(invoice.CarRego, @"[^\w\-]", "_");
                string fileName = $"{safeCarRego}_Invoice_{id}.pdf";

                _logger.LogInformation($"Returning PDF file: {fileName}");
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while generating PDF for invoice {id}");
                return StatusCode(500, "An error occurred while generating the PDF.");
            }
        }

        // POST : Send Invoice PDF to Email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendInvoiceEmail(int invoiceId)
        {
            try
            {
                _logger.LogInformation($"Request to send invoice email for invoice with id {invoiceId}");

                if (invoiceId <= 0)
                {
                    _logger.LogWarning("Invalid invoice ID received.");
                    return Json(new { success = false, message = "Invalid invoice ID." });
                }

                var invoice = await _unitOfWork.Invoice.GetInvoiceDetailsAsync(invoiceId);
                if (invoice == null)
                {
                    _logger.LogWarning($"Invoice not found with id {invoiceId}");
                    return Json(new { success = false, message = "Invoice not found." });
                }

                // Create the PDF from invoice details
                var pdfBytes = _invoicePdfService.CreateInvoicePdf(invoice);

                // Prepare email details
                var subject = $"Invoice #{invoice.InvoiceId} from Mobile Mekaniko";
                var body = $"Dear {invoice.CustomerName},\n\nPlease find attached your invoice #{invoice.InvoiceId}.\n\nThank you,\nMobile Mekaniko";
                var email = invoice.CustomerEmail;

                // Use EmailPdfService to send the invoice email with the PDF attachment
                await _emailPdfService.SendInvoiceEmailAsync(email, subject, body, pdfBytes, $"Invoice_{invoice.InvoiceId}.pdf");

                await _unitOfWork.Invoice.UpdateIsEmailSentAsync(invoiceId, true);

                _logger.LogInformation($"Invoice email successfully sent to {email} for invoice with id {invoiceId}");
                return Json(new { success = true, message = "Invoice email successfully sent." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while sending invoice email for invoice with id {invoiceId}");
                return Json(new { success = false, message = "An error occurred while sending the invoice email." });
            }
        }

        // POST : Mark Invoice as Paid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkInvoiceAsPaid(int id)
        {
            try
            {
                _logger.LogInformation($"Request to mark invoice with id {id} as paid");

                await _unitOfWork.Invoice.MarkAsPaidAsync(id, true);

                _logger.LogInformation($"Invoice with id {id} successfully marked as paid.");
                return Json(new { success = true, message = "Invoice successfully marked as paid." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while marking the invoice with id {id} as paid.");
                return Json(new { success = false, message = "An error occurred while marking the invoice as paid." });
            }
        }

        // GET : Invoice List
        public async Task<IActionResult> GetInvoiceList()
        {
            try
            {
                _logger.LogInformation("Request to fetch invoice list.");

                var invoice = await _unitOfWork.Invoice.GetInvoiceListAsync();

                _logger.LogInformation($"Fetched {invoice.Count} numbers of invoice.");
                return View(invoice);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to fetch invoice list.");
                return StatusCode(500, "An error occurred while trying to fetch invoice list.");
            }
        }

        // GET : Search invoice by rego
        public async Task<IActionResult> SearchInvoiceByRego(string rego)
        {
            try
            {
                _logger.LogInformation($"Request to fetch invoices with rego {rego}");

                var invoice = await _unitOfWork.Invoice.SerachInvoiceByRegoAsync(rego);

                _logger.LogInformation($"Success fully fetched invoice details, returning invoice");
                return View("GetInvoiceList", invoice);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured while fetching invoices with rego # {rego}");

                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";

                return RedirectToAction("GetInvoiceList"); 
            }
        }

        // GET : Paid Invoices
        public async Task<IActionResult> GetPaidInvoices()
        {
            try
            {
                _logger.LogInformation("Request to fetch paid invoices");

                var invoices = await _unitOfWork.Invoice.FilterPaidInvoicesAsync();


                _logger.LogInformation($"{invoices.Count} paid invoice.");
                return View("GetInvoiceList", invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paid invoices");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";
                return RedirectToAction("GetInvoiceList");
            }
        }

        // GET : Unpaid Invoices
        public async Task<IActionResult> GetUnpaidInvoices()
        {
            try
            {
                _logger.LogInformation("Request to fetch unpaid invoices");

                var invoices = await _unitOfWork.Invoice.FilterUnpaidInvoicesAsync();


                _logger.LogInformation($"{invoices.Count} unpaid invoice.");
                return View("GetInvoiceList", invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching unpaid invoices");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";
                return RedirectToAction("GetInvoiceList");
            }
        }

        // GET : Unpaid Invoices
        public async Task<IActionResult> GetUnsentEmail()
        {
            try
            {
                _logger.LogInformation("Request to fetch unsent emails");

                var invoices = await _unitOfWork.Invoice.FilterUnsentEmailAsync();


                _logger.LogInformation($"{invoices.Count} unsent emails.");
                return View("GetInvoiceList", invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching unsent emails");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";
                return RedirectToAction("GetInvoiceList");
            }
        }
    }
}

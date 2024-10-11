using Microsoft.AspNetCore.Mvc;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;
using SQLitePCL;
using System.Drawing.Printing;

namespace MobileMekaniko_Final.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
       
            
        // GET : Dashboard
        public async Task<IActionResult> MyDashboard()
        {
            try
            {
                var totalCustomers = await _unitOfWork.Dashboard.GetTotalCustomersAsync();
                var totalCars = await _unitOfWork.Dashboard.GetTotalCarsAsync();
                var totalInvoices = await _unitOfWork.Dashboard.GetTotalInvoicesAsync();
                var totalQuotations = await _unitOfWork.Dashboard.GetTotalQuotationsAsync();

                // Create a view model to hold dashboard datas.
                var dashboardDto = new DashboardDto
                {
                    TotalCustomers = totalCustomers,
                    TotalCars = totalCars,
                    TotalInvoices = totalInvoices,
                    TotalQuotations = totalQuotations
                };

                _logger.LogInformation("Successfully fetched dashboard data.");
                return View(dashboardDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching dashboard data.");
                TempData["ErrorMessage"] = "An error occurred while processing dashboard data.";
                return View();
            }
        }
    }
}

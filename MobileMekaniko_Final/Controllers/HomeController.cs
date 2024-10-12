using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using MobileMekaniko_Final.Models;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;
using System.Diagnostics;

namespace MobileMekaniko_Final.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Redirect to Dashboard if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyDashboard");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                var recentInvoices = await _unitOfWork.Dashboard.RecentInvoicesAsync();
                var unpaidInvoices = await _unitOfWork.Dashboard.FilterUnpaidInvoicesAsync();

                // Create a view model to hold dashboard datas.
                var dashboardDto = new DashboardDto
                {
                    TotalCustomers = totalCustomers,
                    TotalCars = totalCars,
                    TotalInvoices = totalInvoices,
                    TotalQuotations = totalQuotations,
                    UnpaidInvoices = unpaidInvoices,
                    RecentInvoices = recentInvoices
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

        // GET : Pie Chart 
        public async Task<IActionResult> GetFinancialData()
        {
            try
            {
                var totalInvoicedAmount = await _unitOfWork.Dashboard.GetTotalInvoicedAmountAsync();
                var totalPaidAmount = await _unitOfWork.Dashboard.GetTotalPaidAmountAsync();
                var remainingBalance = await _unitOfWork.Dashboard.GetRemainingBalanceAsync();

                var data = new
                {
                    totalInvoiceAmount = totalInvoicedAmount,
                    totalPaidAmount = totalPaidAmount,
                    remainingBalance = remainingBalance
                };

                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching financial data.");
                return StatusCode(500, "An error occurred while processing financial data.");
            }
        }

        public async Task<IActionResult> GetMonthlyFinancialData()
        {
            try
            {
                // Fetch monthly data for total invoiced amount and total paid amount
                var monthlyData = await _unitOfWork.Dashboard.GetMonthlyFinancialDataAsync();

                // Prepare data for the chart
                var data = new
                {
                    months = monthlyData.Select(d => d.MonthName).ToList(),  // ['January', 'February', ...]
                    totalInvoicedAmounts = monthlyData.Select(d => d.TotalInvoicedAmount).ToList(),
                    totalPaidAmounts = monthlyData.Select(d => d.TotalPaidAmount).ToList()
                };

                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching monthly financial data.");
                return StatusCode(500, "An error occurred while processing financial data.");
            }
        }

        
    }
}


using Microsoft.EntityFrameworkCore;
using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger<DashboardRepository> _logger;

        public DashboardRepository(ApplicationDbContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<DashboardRepository>();
        }

        public async Task<decimal> GetRemainingBalanceAsync()
        {
            try
            {
                var totalInvoicedAmount = await GetTotalInvoicedAmountAsync();
                var totalPaidAmount = await GetTotalPaidAmountAsync();
                var remainingBalance = totalInvoicedAmount - totalPaidAmount;

                _logger.LogInformation($"Successfully calculated remaining balance: {remainingBalance}");
                return remainingBalance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calculating remaining balance.");
                throw;
            }
        }

        public async Task<int> GetTotalCarsAsync()
        {
            try
            {
                // Getting total number of cars
                var totalCars = await _data.Cars.CountAsync();

                _logger.LogInformation($"Successfully fetched {totalCars} cars");
                return totalCars;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching total number of cars.");
                throw;
            }
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            try
            {
                // Getting total number of customers
                var totalCustomer = await _data.Customers.CountAsync();

                _logger.LogInformation($"Successfully fetched {totalCustomer} customers.");
                return totalCustomer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching total number of customers.");
                throw;
            }
        }

        public async Task<decimal> GetTotalInvoicedAmountAsync()
        {
            try
            {
                var totalInvoicedAmount = await _data.Invoices.SumAsync(i => i.TotalAmount ?? 0);
                _logger.LogInformation($"Successfully calculated total invoiced amount: {totalInvoicedAmount}");
                return totalInvoicedAmount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calculating total invoiced amount");
                throw;
            }
        }

        public async Task<int> GetTotalInvoicesAsync()
        {
            try
            {
                // Getting total number of invoices
                var totalInvoices = await _data.Invoices.CountAsync();

                _logger.LogInformation($"Successfully fetched {totalInvoices} invoices.");
                return totalInvoices;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching total number of invoices.");
                throw;
            }
        }

        public async Task<decimal> GetTotalPaidAmountAsync()
        {
            try
            {
                var totalPaidAmount = await _data.Invoices.Where(i => i.IsPaid).SumAsync(i => i.AmountPaid ?? 0);
                _logger.LogInformation($"Successfully calculated total paid amount: {totalPaidAmount}");
                return totalPaidAmount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calculating total paid amount.");
                throw;
            }
        }

        public async Task<int> GetTotalQuotationsAsync()
        {
            try
            {
                // Get total number of quotations
                var totalQuotations = await _data.Quotations.CountAsync();

                _logger.LogInformation($"Successfully fetched {totalQuotations} quotations.");
                return totalQuotations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching total number of quotations.");
                throw;
            }
        }

        public async Task<List<MonthlyFinancialDataDto>> GetMonthlyFinancialDataAsync()
        {
            try
            {
                // Group by month and calculate totals
                var monthlyFinancialData = await _data.Invoices
                    .Where(i => i.DateAdded.HasValue) // Ensure that DateAdded is not null
                    .GroupBy(i => new { Year = i.DateAdded.Value.Year, Month = i.DateAdded.Value.Month })
                    .Select(g => new MonthlyFinancialDataDto
                    {
                        MonthName = g.Key.Month.ToString("00") + "-" + g.Key.Year,  // e.g., '01-2024'
                        TotalInvoicedAmount = g.Sum(i => i.TotalAmount ?? 0),
                        TotalPaidAmount = g.Sum(i => i.AmountPaid ?? 0 )
                    })
                    .ToListAsync();

                return monthlyFinancialData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching monthly financial data.");
                throw;
            }
        }

        public async Task<List<InvoiceListDto>> FilterUnpaidInvoicesAsync()
        {
            try
            {
                var invoices = await _data.Invoices
                    .Include(i => i.Car)
                        .ThenInclude(car => car.Customer)
                    .Where(i => i.IsPaid == false)
                    .OrderByDescending(i => i.DueDate)
                    .Take(5)
                    .Select(i => new InvoiceListDto
                    {
                        InvoiceId = i.InvoiceId,
                        IsPaid = i.IsPaid,
                        IssueName = i.IssueName,
                        CustomerName = i.Car.Customer.CustomerName,
                        CarRego = i.Car.CarRego,
                        DueDate = i.DueDate,
                        TotalAmount = i.TotalAmount,
                        IsEmailSent = i.IsEmailSent
                    }).ToListAsync();


                _logger.LogInformation($"Successfully fetched {invoices.Count} unpaid invoice.");
                return invoices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching unpaid invoices");
                throw;
            }
        }

        public async Task<List<InvoiceListDto>> RecentInvoicesAsync()
        {
            try
            {
                var recentInvoices = await _data.Invoices
                        .Include(i => i.Car)
                            .ThenInclude(car => car.Customer)
                            .OrderByDescending(i => i.DateAdded)
                            .Take(5)
                            .Select(i => new InvoiceListDto
                            {
                                InvoiceId = i.InvoiceId,
                                IsPaid = i.IsPaid,
                                IssueName = i.IssueName,
                                CustomerName = i.Car.Customer.CustomerName,
                                CarRego = i.Car.CarRego,
                                DueDate = i.DueDate,
                                TotalAmount = i.TotalAmount,
                                IsEmailSent = i.IsEmailSent
                            }).ToListAsync();

                _logger.LogInformation($"Successfully fetched recent invoices");
                return recentInvoices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching recent invoices.");
                throw;
            }
        }
    }
}

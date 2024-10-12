using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Repository.IRepository
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalCustomersAsync();
        Task<int> GetTotalCarsAsync();
        Task<int> GetTotalInvoicesAsync();
        Task<int> GetTotalQuotationsAsync();
        Task<decimal> GetTotalInvoicedAmountAsync();
        Task<decimal> GetTotalPaidAmountAsync();
        Task<decimal> GetRemainingBalanceAsync();
        Task<List<MonthlyFinancialDataDto>> GetMonthlyFinancialDataAsync();
        Task<List<InvoiceListDto>> FilterUnpaidInvoicesAsync();
        Task<List<InvoiceListDto>> RecentInvoicesAsync();
    }
}
    
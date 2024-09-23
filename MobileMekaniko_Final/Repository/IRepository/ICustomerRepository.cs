using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Repository.IRepository
{
    public interface ICustomerRepository
    {
        Task<List<CustomerListSummaryDto>> GetCustomersAsync();
        Task<CustomerDetailsDto> GetCustomerDetailsAsync(int id); 
        Task AddCustomerAsync(CustomerDetailsDto dto);
        Task UpdateCustomerAsync(CustomerDetailsDto dto);
        Task DeleteCustomerAsync(int id);
        Task<List<CustomerListSummaryDto>> SearchCustomerByNameAsync(string customerName);
    }
}

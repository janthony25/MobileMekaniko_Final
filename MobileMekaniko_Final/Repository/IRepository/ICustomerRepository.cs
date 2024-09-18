using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Repository.IRepository
{
    public interface ICustomerRepository
    {
        Task<List<CustomerListSummaryDto>> GetCustomersAsync();
        Task AddCustomerAsync(AddCustomerDto dto);
        Task<bool> UpdateCustomerAsync(UpdateDeleteCustomerDto dto);
    }
}

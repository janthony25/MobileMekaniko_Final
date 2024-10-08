using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Repository.IRepository
{
    public interface IInvoiceRepository
    {
        Task<InvoiceDetailsDto> GetInvoiceDetailsAsync(int id);
        Task<InvoiceCustomerCarDetailsDto> GetCustomerCarDetailsAsync(int id);
        Task AddInvoiceAsync(AddInvoiceDto dto);
        Task UpdateInvoiceAsync(UpdateInvoiceDto dto);
        Task DeleteInvoiceAsync(int id);
        Task UpdateIsEmailSentAsync(int id, bool isEmailSent);
        Task MarkAsPaidAsync(int id, bool IsPaid);
        Task<List<InvoiceListDto>> GetInvoiceListAsync();
    }
}

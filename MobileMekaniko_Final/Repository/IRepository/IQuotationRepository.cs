using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Repository.IRepository
{
    public interface IQuotationRepository
    {
        Task<CarQuotationSummaryDto> GetCarQuotationSummaryAsync(int id);
        Task<QuotationDetailsDto> GetQuotationDetailsAsync(int id);
        Task AddQuotationAsync(AddQuotationDto dto);
        Task UpdateQuotationAsync(UpdateQuotationDto dto);
        Task DeleteQuotationAsync(int id);
        Task UpdateIsEmailSendAsync(int id, bool emailSent);
        Task<List<QuotationListDto>> GetQuotationListAsync();
        Task<List<QuotationListDto>> SearchQuotationByRego(string rego);
    }
}

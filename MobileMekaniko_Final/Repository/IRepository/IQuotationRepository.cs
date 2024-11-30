using MobileMekaniko_Final.Helpers;
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
        Task<PaginatedList<QuotationListDto>> GetQuotationListAsync(int pageNumber, int pageSize, string? searchTerm, string? filter = null);
        //Task<List<QuotationListDto>> SearchQuotationByRego(string rego);
        //Task<List<QuotationListDto>> FilterUnsentEmail();
    }
}

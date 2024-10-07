using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Repository.IRepository
{
    public interface IQuotationRepository
    {
        Task<CarQuotationSummaryDto> GetCarQuotationSummaryAsync(int id);
        Task<QuotationDetailsDto> GetQuotationDetailsAsync(int id);
    }
}

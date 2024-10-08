using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Services
{
    public interface IQuotationPdfService
    {
        byte[] CreateQuotationPdf(QuotationDetailsDto quotation);
    }
}

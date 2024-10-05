using MobileMekaniko_Final.Models.Dto;

namespace MobileMekaniko_Final.Services
{
    public interface IInvoicePdfService
    {
        byte[] CreateInvoicePdf(InvoiceDetailsDto invoice);
    }
}

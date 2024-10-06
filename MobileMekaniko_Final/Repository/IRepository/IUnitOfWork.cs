namespace MobileMekaniko_Final.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICustomerRepository Customer { get; }
        ICarRepository Car { get; }
        IInvoiceRepository Invoice { get; }
        IQuotationRepository Quotation { get; } 
    }
}

using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _data;
        private readonly ILoggerFactory _loggerFactory;

        public UnitOfWork(ApplicationDbContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _loggerFactory = loggerFactory;
            Customer = new CustomerRepository(_data, _loggerFactory);
            Car = new CarRepository(_data, _loggerFactory);
            Invoice = new InvoiceRepository(_data, _loggerFactory);
            Quotation = new QuotationRepository(_data, _loggerFactory);
            Make = new MakeRepository(_data, loggerFactory);
        }
        public ICustomerRepository Customer { get; private set; }

        public ICarRepository Car { get; private set; }

        public IInvoiceRepository Invoice { get; private set; }
        public IQuotationRepository Quotation { get; private set; }
        public IMakeRepository Make { get; private set; }
    }
}

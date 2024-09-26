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
        }
        public ICustomerRepository Customer { get; private set; }

        public ICarRepository Car { get; private set; }
    }
}

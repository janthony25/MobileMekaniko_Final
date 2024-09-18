using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger<CustomerRepository> _logger;

        public UnitOfWork(ApplicationDbContext data, ILogger<CustomerRepository> logger)
        {
            _data = data;
            Customer = new CustomerRepository(_data, _logger);
        }
        public ICustomerRepository Customer { get; private set; }
    }
}

using Microsoft.EntityFrameworkCore;
using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(ApplicationDbContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<InvoiceRepository>();
        }

        public async Task<InvoiceCustomerCarDetailsDto> GetCustomerCarDetailsAsync(int id)
        {
            try
            {
                // Get Customer Car details by Car Id
                var customerCar = await _data.Cars
                        .Include(car => car.Customer)
                        .Where(car => car.CarId == id)
                        .Select(car => new InvoiceCustomerCarDetailsDto
                        {
                            CustomerName = car.Customer.CustomerName,
                            CarRego = car.CarRego
                        }).FirstOrDefaultAsync();

                _logger.LogInformation($"Successfully fetched customer and car details for car with id {id}");
                return customerCar;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching customer and car details for car with id {id}");
                throw;
            }
        }

        public async Task<InvoiceDetailsDto> GetInvoiceDetailsAsync(int id)
        {
            try
            {
                // find invoice details by invoice id
                var invoice = await _data.Invoices
                    .Include(i => i.Car)
                        .ThenInclude(car => car.Customer)
                    .Include(i => i.InvoiceItem)
                    .Where(i => i.InvoiceId == id)
                    .Select(i => new InvoiceDetailsDto
                    {
                        CustomerName = i.Car.Customer.CustomerName,
                        CarRego = i.Car.CarRego,
                        DateAdded = i.DateAdded,
                        DueDate = i.DueDate,
                        IssueName = i.IssueName,
                        PaymentTerm = i.PaymentTerm,
                        Notes = i.Notes,
                        LabourPrice = i.LaborPrice,
                        Discount = i.Discount,
                        ShippingFee = i.ShippingFee,
                        SubTotal = i.SubTotal,
                        TotalAmount = i.TotalAmount,
                        AmountPaid = i.AmountPaid,
                        isPaid = i.IsPaid,
                        InvoiceItemDto = i.InvoiceItem.Select(ii => new InvoiceItemDto
                        {
                            InvoiceItemId = ii.InvoiceItemId,
                            Quantity = ii.Quantity,
                            ItemPrice = ii.ItemPrice,
                            ItemTotal = ii.ItemPrice
                        }).ToList()
                    }).FirstOrDefaultAsync();

                if(invoice == null || invoice.InvoiceId == 0)
                {
                    _logger.LogInformation("No invoice details found.");
                    return invoice;
                }

                _logger.LogInformation($"Success fully found invoice details for invoice with id {id}");
                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting invoice details of invoice with id {id}");
                throw;
            }
                    
        }
    }
}

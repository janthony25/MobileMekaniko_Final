using Microsoft.EntityFrameworkCore;
using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Repository
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger<QuotationRepository> _logger;
        public QuotationRepository(ApplicationDbContext data, ILoggerFactory loggerFactory)
        {
            _data = data;
            _logger = loggerFactory.CreateLogger<QuotationRepository>();
        }

        public async Task<CarQuotationSummaryDto> GetCarQuotationSummaryAsync(int id)
        {
            try
            {
                // Find Car by id
                var car = await _data.Cars
                    .Include(c => c.Customer)
                    .Include(c => c.CarMake)
                        .ThenInclude(cm => cm.Make)
                    .Include(c => c.Quotation)
                    .Where(c => c.CarId == id)
                    .Select(c => new CarQuotationSummaryDto
                    {
                        CustomerId = c.Customer.CustomerId,
                        CustomerName = c.Customer.CustomerName,
                        CustomerEmail = c.Customer.CustomerEmail,
                        CustomerNumber = c.Customer.CustomerNumber,
                        CarId = c.CarId,
                        CarRego = c.CarRego,
                        MakeName = c.CarMake.FirstOrDefault().Make.MakeName,
                        CarModel = c.CarModel,
                        CarYear = c.CarYear,
                        Quotations = c.Quotation.Select(q => new QuotationSummaryDto
                        {
                            QuotationId = q.CarId,
                            IssueName = q.IssueName,
                            DateAdded = q.DateAdded,
                            TotalAmount = q.TotalAmount
                        }).ToList()
                    }).FirstOrDefaultAsync();

                _logger.LogInformation($"Car quotation details with car id {id} fetched successfully.");
                return car;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching car details of car with id {id}");
                throw;
            }
        }
    }
}

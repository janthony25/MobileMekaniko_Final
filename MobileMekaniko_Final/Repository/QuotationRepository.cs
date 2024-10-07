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

        public async Task<QuotationDetailsDto> GetQuotationDetailsAsync(int id)
        {
            try
            {
                // Find Quote by id
                var quote = await _data.Quotations
                    .Include(q => q.Car)
                        .ThenInclude(car => car.Customer)
                    .Include(q => q.Car)
                        .ThenInclude(car => car.CarMake)
                            .ThenInclude(cm => cm.Make)
                    .Include(q => q.QuotationItem)
                    .Where(q => q.QuotationId == id)
                    .Select(q => new QuotationDetailsDto
                    {
                        CustomerId = q.Car.Customer.CustomerId,
                        CustomerName = q.Car.Customer.CustomerAddress,
                        CustomerEmail = q.Car.Customer.CustomerEmail,
                        CustomerNumber = q.Car.Customer.CustomerNumber,
                        CarId = q.Car.CarId,
                        CarRego = q.Car.CarRego,
                        MakeName = q.Car.CarMake.FirstOrDefault().Make.MakeName,
                        CarModel = q.Car.CarModel,
                        CarYear = q.Car.CarYear,
                        QuotationId = q.QuotationId,
                        DateAdded = q.DateAdded,
                        DateEdited = q.DateEdited,
                        IssueName = q.IssueName,
                        Notes = q.Notes,
                        LaborPrice = q.LaborPrice,
                        Discount = q.Discount,
                        ShippingFee = q.ShippingFee,
                        SubTotal = q.SubTotal,
                        TotalAmount = q.TotalAmount,
                        IsEmailSent = q.IsEmailSent,
                        QuotationItemDto = q.QuotationItem.Select(qi => new QuotationItemDto
                        {
                            QuotationItemId = qi.QuotationItemId,
                            ItemName = qi.ItemName,
                            Quantity = qi.Quantity,
                            ItemPrice = qi.ItemPrice,
                            ItemTotal = qi.ItemTotal
                        }).ToList()
                    }).FirstOrDefaultAsync();

                if(quote == null)
                {
                    _logger.LogInformation("No quotation details found.");
                    return quote;
                }

                _logger.LogInformation($"Successfully fetched quotation details for quotation with id {id}");
                return quote;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching quotation details for quotation with id {id}");
                throw;
            }
        }
    }
}

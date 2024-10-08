﻿using Microsoft.EntityFrameworkCore;
using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Models;
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

        public async Task AddQuotationAsync(AddQuotationDto dto)
        {
            try
            {
                // Find Car by dto.CarId
                var car = await _data.Cars.FindAsync(dto.CarId);

                var quotation = new Quotation
                {
                    IssueName = dto.IssueName,
                    Notes = dto.Notes,
                    LaborPrice = dto.LaborPrice,
                    Discount = dto.Discount,
                    ShippingFee = dto.ShippingFee,
                    SubTotal = dto.SubTotal,
                    TotalAmount = dto.TotalAmount,
                    CarId = dto.CarId
                };

                _data.Quotations.Add(quotation);
                _logger.LogInformation($"Successfully added quotation to car with id {dto.CarId}");

                await _data.SaveChangesAsync();

                var quotationItem = dto.QuotationItems.Select(qi => new QuotationItem
                {
                    ItemName = qi.ItemName,
                    Quantity = qi.Quantity,
                    ItemPrice = qi.ItemPrice,
                    ItemTotal = qi.ItemTotal,
                    QuotationId = quotation.QuotationId
                }).ToList();

                _data.QuotationItems.AddRange(quotationItem);
                _logger.LogInformation($"Successfully added items to quotation with id {quotation.QuotationId}");

                await _data.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding quotation to car with id {dto.CarId}");
                throw;
            }
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
                            QuotationId = q.QuotationId,
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
                        CustomerName = q.Car.Customer.CustomerName,
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

        public async Task UpdateQuotationAsync(UpdateQuotationDto dto)
        {
            try
            {
                // Find Quotation by id
                var quotation = await _data.Quotations.FindAsync(dto.QuotationId);

                if(quotation == null)
                {
                    _logger.LogWarning($"No details found for quotation with {dto.QuotationId}");
                    throw new KeyNotFoundException($"No details found for quotation with id {dto.QuotationId}");
                }

                quotation.IssueName = dto.IssueName;
                quotation.Notes = dto.Notes;
                quotation.LaborPrice = dto.LaborPrice;
                quotation.Discount = dto.Discount;
                quotation.ShippingFee = dto.ShippingFee;
                quotation.SubTotal = dto.SubTotal;
                quotation.TotalAmount = dto.TotalAmount;
                quotation.DateEdited = DateTime.Now;

                await _data.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated quotation with id {dto.QuotationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to update quotation with id {dto.QuotationId}");
                throw;
            }
        }
    }
}

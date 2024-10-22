using Microsoft.EntityFrameworkCore;
using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Models;
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

        public async Task AddInvoiceAsync(AddInvoiceDto dto)
        {
            try
            {
                // Fetching CarId
                var car = await _data.Cars.FindAsync(dto.CarId);

                // Creating new Invoice
                var invoice = new Invoice
                {
                    IssueName = dto.IssueName,
                    DateAdded = dto.DateAdded,
                    DueDate = dto.DueDate,
                    PaymentTerm = dto.PaymentTerm,
                    Notes = dto.Notes,
                    LaborPrice = dto.LaborPrice,
                    Discount = dto.Discount,
                    ShippingFee = dto.ShippingFee,
                    SubTotal = dto.SubTotal,
                    TaxAmount = dto.TaxAmount,
                    TotalAmount = dto.TotalAmount,
                    AmountPaid = dto.AmountPaid,
                    IsPaid = dto.IsPaid,
                    CarId = car.CarId
                };

                _logger.LogInformation($"Adding invoice details to car with id {dto.CarId}");
                _data.Invoices.Add(invoice);

                // Save changes
                await _data.SaveChangesAsync();

                // add new InvoiceItem 
                var invoiceItem = dto.InvoiceItems.Select(item => new InvoiceItem
                {
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    ItemPrice = item.ItemPrice,
                    ItemTotal = item.ItemTotal,
                    InvoiceId = invoice.InvoiceId
                }).ToList();

                _logger.LogInformation($"Adding {invoiceItem.Count} invoice items to invoice with id {invoice.InvoiceId}");
                _data.InvoiceItems.AddRange(invoiceItem);

                await _data.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding invoice.");
                throw;
            }


        }

        public async Task DeleteInvoiceAsync(int id)
        {
            try
            {
                // Find Invoice by id
                var invoice = await _data.Invoices.FindAsync(id);

                if (invoice == null)
                {
                    _logger.LogWarning("No invoice found.");
                    throw new KeyNotFoundException($"No invoice found with id {id}");
                }

                // Remove invoice 
                _data.Invoices.Remove(invoice);
                await _data.SaveChangesAsync();
                _logger.LogInformation($"Invoice with id {id} successfully deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting invoice with id {id}");
                throw;
            }
        }

        public async Task<List<InvoiceListDto>> FilterPaidInvoicesAsync()
        {
            try
            {
                var invoices = await _data.Invoices
                    .Include(i => i.Car)
                        .ThenInclude(car => car.Customer)
                    .Where(i => i.IsPaid == true)
                    .OrderByDescending(i => i.DueDate)
                    .Select(i => new InvoiceListDto
                    {
                        InvoiceId = i.InvoiceId,
                        IsPaid = i.IsPaid,
                        IssueName = i.IssueName,
                        CustomerName = i.Car.Customer.CustomerName,
                        CarRego = i.Car.CarRego,
                        DueDate = i.DueDate,
                        TotalAmount = i.TotalAmount,
                        IsEmailSent = i.IsEmailSent
                    }).ToListAsync();

                if(invoices == null || invoices.Count == 0)
                {
                    return await GetInvoiceListAsync();
                }

                _logger.LogInformation($"Successfully fetched {invoices.Count} paid invoice.");
                return invoices;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paid invoices");
                throw;
            }
        }

        public async Task<List<InvoiceListDto>> FilterUnpaidInvoicesAsync()
        {
            try
            {
                var invoices = await _data.Invoices
                    .Include(i => i.Car)
                        .ThenInclude(car => car.Customer)
                    .Where(i => i.IsPaid == false)
                    .OrderByDescending(i => i.DueDate)
                    .Select(i => new InvoiceListDto
                    {
                        InvoiceId = i.InvoiceId,
                        IsPaid = i.IsPaid,
                        IssueName = i.IssueName,
                        CustomerName = i.Car.Customer.CustomerName,
                        CarRego = i.Car.CarRego,
                        DueDate = i.DueDate,
                        TotalAmount = i.TotalAmount,
                        IsEmailSent = i.IsEmailSent
                    }).ToListAsync();

                if (invoices == null || invoices.Count == 0)
                {
                    return await GetInvoiceListAsync();
                }

                _logger.LogInformation($"Successfully fetched {invoices.Count} unpaid invoice.");
                return invoices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching unpaid invoices");
                throw;
            }
        }

        public async Task<List<InvoiceListDto>> FilterUnsentEmailAsync()
        {
            try
            {
                var invoices = await _data.Invoices
                    .Include(i => i.Car)
                        .ThenInclude(car => car.Customer)
                    .Where(i => i.IsEmailSent == false)
                    .OrderByDescending(i => i.DueDate)
                    .Select(i => new InvoiceListDto
                    {
                        InvoiceId = i.InvoiceId,
                        IsPaid = i.IsPaid,
                        IssueName = i.IssueName,
                        CustomerName = i.Car.Customer.CustomerName,
                        CarRego = i.Car.CarRego,
                        DueDate = i.DueDate,
                        TotalAmount = i.TotalAmount,
                        IsEmailSent = i.IsEmailSent
                    }).ToListAsync();

                if (invoices == null || invoices.Count == 0)
                {
                    return await GetInvoiceListAsync();
                }

                _logger.LogInformation($"Successfully fetched {invoices.Count} unsent email.");
                return invoices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching paid invoices");
                throw;
            }
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
                            CarId = car.CarId,
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
                        CustomerAddress = i.Car.Customer.CustomerAddress,
                        CustomerEmail = i.Car.Customer.CustomerEmail,
                        CustomerNumber = i.Car.Customer.CustomerNumber,
                        CarId = i.Car.CarId,
                        CarRego = i.Car.CarRego,
                        MakeName = i.Car.CarMake.FirstOrDefault().Make.MakeName,
                        CarModel = i.Car.CarModel,
                        CarYear = i.Car.CarYear,
                        InvoiceId = i.InvoiceId,
                        DateAdded = i.DateAdded,
                        DueDate = i.DueDate,
                        IssueName = i.IssueName,
                        PaymentTerm = i.PaymentTerm,
                        Notes = i.Notes,
                        LabourPrice = i.LaborPrice,
                        Discount = i.Discount,
                        ShippingFee = i.ShippingFee,
                        SubTotal = i.SubTotal,
                        TaxAmount = i.TaxAmount,
                        TotalAmount = i.TotalAmount,
                        AmountPaid = i.AmountPaid,
                        isPaid = i.IsPaid,
                        InvoiceItemDto = i.InvoiceItem.Select(ii => new InvoiceItemDto
                        {
                            InvoiceItemId = ii.InvoiceItemId,
                            ItemName = ii.ItemName,
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

                _logger.LogInformation($"Successfully found invoice details for invoice with id {id}");
                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting invoice details of invoice with id {id}");
                throw;
            }
                    
        }

        public async Task<List<InvoiceListDto>> GetInvoiceListAsync()
        {
            try
            {
                // Get invoice list
                var invoice = await _data.Invoices
                    .Include(i => i.Car)
                        .ThenInclude(car => car.Customer)
                    .OrderByDescending(i => i.DateAdded)
                    .Select(i => new InvoiceListDto
                    {
                        CustomerName = i.Car.Customer.CustomerName,
                        CarRego = i.Car.CarRego,
                        InvoiceId = i.InvoiceId,
                        IssueName = i.IssueName,
                        DateAdded = i.DateAdded,
                        DueDate = i.DueDate,
                        TotalAmount = i.TotalAmount,
                        AmountPaid = i.AmountPaid,
                        IsEmailSent = i.IsEmailSent,
                        IsPaid = i.IsPaid
                    }).ToListAsync();

                _logger.LogInformation($"Invoices fetched: {invoice.Count}");
                return invoice;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching invoice list.");
                throw;
            }
        }

        public async Task MarkAsPaidAsync(int id, bool IsPaid)
        {
            try
            {
                // Find invoice by id
                var invoice = await _data.Invoices.FindAsync(id);

                if(invoice == null)
                {
                    _logger.LogWarning($"No invoice found with id {id}");
                    throw new KeyNotFoundException("No invoice found.");
                }

                invoice.IsPaid = IsPaid;
                invoice.AmountPaid = invoice.TotalAmount;
                invoice.DateEdited = DateTime.Now;

                await _data.SaveChangesAsync();
                _logger.LogInformation($"Invoice with id {id} marked as paid successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while marking the invoice with id {id} as paid");
                throw;
            }
        }

        public async Task<List<InvoiceListDto>> SerachInvoiceByRegoAsync(string rego)
        {
            try
            {
                if (string.IsNullOrEmpty(rego))
                {
                    return await GetInvoiceListAsync();
                }

                // Fetching invoices by rego
                var invoice = await _data.Invoices
                    .Include(i => i.Car)
                        .ThenInclude(car => car.Customer)
                    .Where(i => i.Car.CarRego.Contains(rego))
                    .Select(i => new InvoiceListDto
                    {
                        InvoiceId = i.InvoiceId,
                        IsPaid = i.IsPaid,
                        IssueName = i.IssueName,
                        CustomerName = i.Car.Customer.CustomerName,
                        CarRego = i.Car.CarRego,
                        DueDate = i.DueDate,
                        TotalAmount = i.TotalAmount,
                        IsEmailSent = i.IsEmailSent
                    }).ToListAsync();

                _logger.LogInformation($"Fetched {invoice.Count} invoices with rego {rego}");
                return invoice;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching invoices with rego {rego}");
                throw;
            }
        }

        public async Task UpdateInvoiceAsync(UpdateInvoiceDto dto)
        {
            try
            {
                // Find Invoice by id
                var invoice = await _data.Invoices.FindAsync(dto.InvoiceId);

                if(invoice == null)
                {
                    _logger.LogWarning($"No details found with invoice id {dto.InvoiceId}");
                    throw new KeyNotFoundException($"Invoice with id {dto.InvoiceId} not found.");
                }

                invoice.DueDate = dto.DueDate;
                invoice.IssueName = dto.IssueName;
                invoice.PaymentTerm = dto.PaymentTerm;
                invoice.Notes = dto.Notes;
                invoice.LaborPrice = dto.LaborPrice;
                invoice.Discount = dto.Discount;
                invoice.ShippingFee = dto.ShippingFee;
                invoice.SubTotal = dto.SubTotal;
                invoice.TaxAmount = dto.TaxAmount;
                invoice.TotalAmount = dto.TotalAmount;
                invoice.AmountPaid = dto.AmountPaid;
                invoice.IsPaid = dto.IsPaid;
                invoice.DateEdited = DateTime.Now;

                await _data.SaveChangesAsync();
                _logger.LogInformation($"Invoice with id {dto.InvoiceId} has been updated successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating invoice with id {dto.InvoiceId}");
                throw;
            }
        }

        public async Task UpdateIsEmailSentAsync(int id, bool isEmailSent)
        {
            try
            {
                // Find Invoice by id
                var invoice = await _data.Invoices.FindAsync(id);
                if(invoice == null)
                {
                    _logger.LogWarning($"No invoice found with id {id}");
                    throw new KeyNotFoundException($"Invoice with id {id} not found.");
                }

                invoice.IsEmailSent = isEmailSent;

                await _data.SaveChangesAsync();
                _logger.LogInformation($"IsEmailSent updated to {isEmailSent} for invoice with id {id}");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating IsEmailSent for invoice with id {id}");
                throw;
            }
        }
    }
}

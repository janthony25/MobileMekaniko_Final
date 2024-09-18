using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using MobileMekaniko_Final.Data;
using MobileMekaniko_Final.Models;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;
using NuGet.Protocol;

namespace MobileMekaniko_Final.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _data;
        private readonly ILogger _logger;

        public CustomerRepository(ApplicationDbContext data, ILogger<CustomerRepository> logger)
        {
            _data = data;
            _logger = logger;
        }

        public async Task AddCustomerAsync(AddCustomerDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Customer data cannot be null");
            }

            var customer = new Customer
            {
                CustomerName = dto.CustomerName,
                CustomerAddress = dto.CustomerAddress,
                CustomerEmail = dto.CustomerEmail,
                CustomerNumber = dto.CustomerNumber
            };

            try
            {
                _data.Customers.Add(customer);
                _data.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the customer");
            }
        }

        public async Task<List<CustomerListSummaryDto>> GetCustomersAsync()
        {
            return await _data.Customers
                .Select(c => new CustomerListSummaryDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    CustomerEmail = c.CustomerEmail,
                    CustomerNumber = c.CustomerNumber
                }).ToListAsync();
        }

        public async Task<bool> UpdateCustomerAsync(UpdateDeleteCustomerDto dto)
        {
            // Find customer using ID
            var customer = await _data.Customers.FindAsync(dto.CustomerId);

            if (customer == null)
            {
                // Log customer that was not found
                _logger.LogWarning($"Customer with ID {dto.CustomerId} not found.");
                return false;
            }

            try
            {
                customer.CustomerName = dto.CustomerName;
                customer.CustomerAddress = dto.CustomerAddress;
                customer.CustomerEmail = dto.CustomerEmail;
                customer.CustomerNumber = dto.CustomerNumber;
                customer.DateEdited = DateTime.Now;

                await _data.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the customer");
                return false;
            }


        }
    }
}

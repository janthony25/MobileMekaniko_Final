
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Execution;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(IUnitOfWork unitOfWork, ILogger<CustomerController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET : Customer List page
        public IActionResult Index()
        {
            return View();
        }

        // GET : Populate Customer table
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                _logger.LogInformation("Received request to fetch customer list.");

                var customer = await _unitOfWork.Customer.GetCustomersAsync();

             
                _logger.LogInformation($"Successfully retrieved {customer.Count} customers.");
                return Json(customer);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching customers.");
                return Json(new { success = false, message = "An error occurred while trying to fetch customers." });
            }
        }


        // GET: Show Customer Details
        public async Task<IActionResult> GetCustomerDetails(int id)
        {
            try
            {
                var customer = await _unitOfWork.Customer.GetCustomerDetailsAsync(id);
                return Json(customer);
            }
            catch
            {
                _logger.LogWarning($"An error occurred while fetching customer details with id: {id}");
                return Json(new { success = false, message = "An error occurred while fetching customer details" });
            }
        }

        // POST : Add Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomer(CustomerDetailsDto dto)
        {
            try
            {
                _logger.LogInformation($"Request to add: {dto.CustomerName}");

                if(ModelState.IsValid)
                {
                    await _unitOfWork.Customer.AddCustomerAsync(dto);
                    return Json(new { success = true, message = "Successfully added a customer." });
                }

                _logger.LogWarning("Invalid model state");
                return Json(new { success = false, message = "Unable to add customer. Invalid data provided" });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while adding {dto.CustomerName}.");
                return Json(new { success = false, message = "An unexpected error occurred while adding customer." });
            }
        }

        // POST : Updating Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomer(CustomerDetailsDto dto)
        {
            try
            {
                _logger.LogInformation($"Request to update customer {dto.CustomerName}");

                if (ModelState.IsValid)
                {
                    await _unitOfWork.Customer.UpdateCustomerAsync(dto);
                    return Json(new { success = true, message = "Customer updated successfully!" });
                }

                _logger.LogWarning("Invalid modelstate");
                return Json(new { success = false, message = "Unable to update customer. Invalid data provided" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while updating customer {dto.CustomerName} with id: {dto.CustomerId} ");
                return Json(new { success = false, message = "Something went wrong while updating customer." });
            }
        }

        // POST : Delete Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                _logger.LogInformation($"Request to delete customer with id {id}");

                await _unitOfWork.Customer.DeleteCustomerAsync(id);
                return Json(new { success = true, message = "Customer deleted successfully." });
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, $"Something went wrong while trying to delete customer with id {id}");
                return Json(new { success = false, message = "Something went wrong while deleting customer." });
            }
        }

        // GET : Search Customer
        public async Task<IActionResult> SearchCustomers(string customerName)
        {
            try
            {
                _logger.LogInformation($"Request to search for customers with {customerName} in their name.");

                var customers = await _unitOfWork.Customer.SearchCustomerByNameAsync(customerName);

                if (customers == null || customers.Count ==0)
                {
                    _logger.LogInformation($"No customers found with the name: {customerName}");
                    return Json(new { success = false, message = "No customer found." });
                }
                _logger.LogInformation($"Found {customers.Count} customers with {customerName}.");
                return Json(new {success=true,customers = customers});
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while searching customers with the name {customerName}");
                return Json(new { success = false, message = "An error occurred while searching for the customer name." });
            }
        }


        // GET : Customer Cars
        public async Task<IActionResult> GetCustomerCars(int id)
        {
            try
            {
                _logger.LogInformation("Request to fetch customer car details.");

                var customerCar = await _unitOfWork.Customer.GetCustomerCarsByIdAsync(id);
                var makes = await _unitOfWork.Car.GetMakesAsync();
                ViewBag.Makes = makes;

                _logger.LogInformation("Successfully fetched customer car details. Returning customer car details");
                return View(customerCar);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching customer car details.");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}

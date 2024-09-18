using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Execution;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CustomerController(IUnitOfWork unitOfWork, ILogger<CustomerController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        // GET : Pupulate Customer table
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customer = await _unitOfWork.Customer.GetCustomersAsync();
                return Json(customer);
            }
            catch (Exception ex)
            {
                return Json(new {success= false, message = "Error occurred while getting customer data."});
            }
        }

        // POST : Add customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomer(AddCustomerDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _unitOfWork.Customer.AddCustomerAsync(dto);
                    return Json(new { success = true, message = "Customer addedd successfully!" });
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Error occurred while adding customer");

                    return Json(new { success = false, message = "Unable to add customer." });
                }
            }
            
            return Json(new { success = false, messsage = "An error occurred while adding customer." });
        }

        // POST : Update customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomer(UpdateDeleteCustomerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            try
            {
                var result = await _unitOfWork.Customer.UpdateCustomerAsync(dto);

                if (result)
                {
                    return Json(new { success = true, message = "Customer updated successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Update failed." });
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating customer");
                return Json(new { success = false, messsage = "An error occurred while updating customer." });
            }
        }
    }
}

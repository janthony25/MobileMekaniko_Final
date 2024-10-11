using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MobileMekaniko_Final.Models.Dto;
using MobileMekaniko_Final.Repository.IRepository;
using SQLitePCL;

namespace MobileMekaniko_Final.Controllers
{
    public class MakeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MakeController> _logger;

        public MakeController(IUnitOfWork unitOfWork, ILogger<MakeController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        

        public async Task<IActionResult> AddRemoveMake()
        {
            try
            {
                var makes = await _unitOfWork.Make.GetMakeListAsync();
                ViewBag.Makes = new SelectList(makes, "MakeId", "MakeName");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing your request.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                return View();
            }
        }

        // POST : Add New Car Make
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMake(AddMakeDto dto)
        {
            try
            {
                _logger.LogInformation($"Request to add {dto.MakeName} to the database");

                if (ModelState.IsValid)
                {
                    await _unitOfWork.Make.AddMakeAsync(dto);
                    _logger.LogInformation($"Successfully added {dto.MakeName} to the database");
                    TempData["SuccessMessage"] = "New make successfully added.";
                    return RedirectToAction("AddRemoveMake");
                }

                _logger.LogInformation("Invalid data provided.");
                TempData["InvalidDataMessage"] = "Please submit a valid Car Make.";
                return RedirectToAction("AddRemoveMake", dto);
            }
            catch (InvalidOperationException)
            {
                _logger.LogInformation("Invalid data provided.");
                TempData["MakeExists"] = "Car make already exists.";
                return RedirectToAction("AddRemoveMake", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to add {dto.MakeName} to the database.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                return View();
            }
        }

        // POST : Remove Car Make
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMake(int id)
        {
            try
            {
                _logger.LogInformation($"Request to remove car make with id {id}");

                await _unitOfWork.Make.DeleteMakeAsync(id);
                _logger.LogInformation($"Car make successfully removed from the database");
                TempData["DeleteMessage"] = "Car make successfully deleted.";
                return RedirectToAction("AddRemoveMake");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while trying to delete car make with id {id}");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";

                return RedirectToAction("AddRemoveMake");
            }
        }
    }
}

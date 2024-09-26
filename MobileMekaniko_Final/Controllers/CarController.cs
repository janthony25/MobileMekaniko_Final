using Microsoft.AspNetCore.Mvc;
using MobileMekaniko_Final.Repository;
using MobileMekaniko_Final.Repository.IRepository;

namespace MobileMekaniko_Final.Controllers
{
    public class CarController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CarRepository> _logger;

        public CarController(IUnitOfWork unitOfWork, ILogger<CarRepository> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;   
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

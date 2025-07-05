using Microsoft.AspNetCore.Mvc;
using MotiZone.Models.ViewModels;
using MotiZone.Services;
using System.Threading.Tasks;

namespace MotiZone.Controllers
{
    /// <summary>
    /// Handles requests related to the Shop view, including vehicle and part browsing.
    /// </summary>
    public class ShopController : Controller
    {
        private readonly VehicleService _vehicleService;
        private readonly PartService _partService;

        public ShopController(VehicleService vehicleService, PartService partService)
        {
            _vehicleService = vehicleService;
            _partService = partService;
        }

        /// <summary>
        /// Displays the shop index view with filtered vehicles and all available parts.
        /// </summary>
        /// <param name="shopId">Optional ID of a specific shop</param>
        /// <param name="make">Vehicle make to filter</param>
        /// <param name="model">Vehicle model to filter</param>
        /// <param name="year">Vehicle year to filter</param>
        [HttpGet]
        public async Task<IActionResult> Index(int? shopId, string? make, string? model, int? year)
        {
            var vehicles = await _vehicleService.GetFilteredVehiclesAsync(make, model, year);
            var parts = await _partService.GetPartsAsync();

            var viewModel = new ShopViewModel
            {
                ShopId = shopId,
                Make = make,
                Model = model,
                Year = year,
                Vehicles = vehicles,
                Parts = parts
            };

            return View(viewModel);
        }
    }
}
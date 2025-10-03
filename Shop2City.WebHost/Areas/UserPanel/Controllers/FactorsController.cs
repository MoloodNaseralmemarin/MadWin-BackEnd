using MadWin.Application.Repositories;
using MadWin.Application.Services;
using MadWin.Core.DTOs.Fators;
using MadWin.Core.DTOs.FilterParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Shop2City.WebHost.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class FactorsController : Controller
    {
        private readonly IFactorDetailService _factorDetailService;
        private readonly IDeliveryMethodService _deliveryMethodService;

        public FactorsController(IFactorDetailService factorDetailService, IDeliveryMethodService deliveryMethodService)
        {
            _factorDetailService = factorDetailService;
            _deliveryMethodService = deliveryMethodService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(FilterParameter filter, int pageId = 1)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();
            var result = await _factorDetailService.GetAllFactorByUserIdAsync(userId,filter, pageId);
            return View(result);
        }


        public async Task<IActionResult> ShowFactorDetails(int id)
        {
            var getAllOrderDetails = await _factorDetailService.GetByFactorIdAsync(id);
            return PartialView("_FactorDetailsPartial", getAllOrderDetails);

        }

        public async Task<IActionResult> FactorSummary(int factorId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();
            var factorSummary = await _factorDetailService.GetOpenFactorAsync(userId,factorId);  // از پارامتر استفاده شد
            var deliveryMethods = await _deliveryMethodService.GetDeliveryMethodInfoAsync();
            if (factorSummary == null)
                return NotFound();

            var viewModel = new FactorSummaryViewModel
            {
                FactorSummaryForAdmin = factorSummary,
                DeliveryMethods = deliveryMethods
            };

            return View(viewModel);
        }
    }
}

using MadWin.Application.Repositories;
using MadWin.Application.Services;
using MadWin.Core.DTOs.Factors;
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
        private readonly IFactorService _factorService;

        public FactorsController(IFactorDetailService factorDetailService, IDeliveryMethodService deliveryMethodService, IFactorService factorService)
        {
            _factorDetailService = factorDetailService;
            _deliveryMethodService = deliveryMethodService;
            _factorService = factorService;

        }
        [HttpGet]
        public async Task<IActionResult> Index(FactorFilterParameter filter, int pageId = 1)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();
            var result = await _factorDetailService.GetAllFactorByUserIdAsync(userId,filter, pageId);
            return View(result);
        }

        #region حذف آیتم از فاکتور
        public async Task<IActionResult> RemoveItemsByFactorAsync(int factorId, int[] factorDetailIds)
        {
            if (factorDetailIds == null || !factorDetailIds.Any())
                return Json(new { success = false });

            await _factorDetailService.SoftDeleteAsync(factorDetailIds);

            // حالا می‌توانیم اطلاعات فاکتور را دوباره بگیریم
            var updatedList = await FactorSummary(factorId);

            return Json(new { success = true, data = updatedList });
        }
        #endregion
        public async Task<IActionResult> ShowFactorDetails(int id)
        {
            var getAllOrderDetails = await _factorDetailService.GetByFactorIdAsync(id);
            return PartialView("_FactorDetailsPartial", getAllOrderDetails);

        }

        public async Task<IActionResult> FactorSummary(int? factorId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            if(!factorId.HasValue)
            {
                factorId=await _factorService.GetLastFactorId(userId);
            }
             
            var factorSummary = await _factorDetailService.GetOpenFactorAsync(userId,factorId);  // از پارامتر استفاده شد
            var deliveryMethods = await _deliveryMethodService.GetDeliveryMethodInfoAsync();
            if (factorSummary == null)
                return NotFound();

            var viewModel = new FactorSummaryViewModel
            {
                FactorSummaryForAdmin = factorSummary,
                DeliveryMethods = deliveryMethods
            };

            // بعد از لود، آیتم‌های جدید رو به حالت دیده‌شده تغییر بده
            await _factorDetailService.MarkItemsAsSeenAsync(factorId);
            return View(viewModel);
        }
    }
}

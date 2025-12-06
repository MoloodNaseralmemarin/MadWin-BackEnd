using MadWin.Application.DTOs.Products;
using MadWin.Application.Services;
using MadWin.Core.DTOs.DeliveryMethods;
using MadWin.Core.Entities.DeliveryMethods;
using MadWin.Core.Entities.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class DeliveryMethodsController : Controller
    {
        private readonly IDeliveryMethodService _deliveryMethodService;
        public DeliveryMethodsController(IDeliveryMethodService deliveryMethodService)
        {
            _deliveryMethodService = deliveryMethodService;
        }
        public async Task<IActionResult> Index()
        {
            var allDeliveryMethods = await _deliveryMethodService.GetDeliveryMethodForAdminAsync();
            return View(allDeliveryMethods);
        }

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var deliveryMethod = await _deliveryMethodService.GetDeliveryMethodByIdAsync(id);
            if (deliveryMethod == null)
            {
                return NotFound();
            }

            var vm = new EditDeliveryMethodForAdmin
            {
                Id = id,
                Cost = deliveryMethod.Cost,
                Name= deliveryMethod.Name
            };

            return View(deliveryMethod);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditDeliveryMethodForAdmin editDeliveryMethod)
        {
            if (!ModelState.IsValid)
                return View(editDeliveryMethod);
            var item = new DeliveryMethod
            {
                Id = editDeliveryMethod.Id,
                Name = editDeliveryMethod.Name,
            };

            var success = await _deliveryMethodService.EditDeliveryMethodAsync(editDeliveryMethod);

            if (!success)
            {
                ModelState.AddModelError("", "خطایی در بروزرسانی اطلاعات شیوه ارسال رخ داد.");
                return View(editDeliveryMethod);
            }

            TempData["Success"] = "اطلاعات شیوه ارسال با موفقیت بروزرسانی شد.";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}

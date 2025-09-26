using MadWin.Application.Services;
using MadWin.Core.Entities.CurtainComponents;
using Microsoft.AspNetCore.Mvc;
using Shop2City.WebHost.ViewModels.CurtainComponents;
using System.Threading.Tasks;

namespace Shop2City.WebHost.Controllers
{
    public class CurtainComponentsController : Controller
    {
        public readonly ICurtainComponentService _curtainComponentService;

        public CurtainComponentsController(ICurtainComponentService curtainComponentService)
        {
            _curtainComponentService = curtainComponentService;
        }
        public async Task<IActionResult> Index()
        {
            var curtainComponent = await _curtainComponentService.GetAllCurtainComponentAsync();
            return View(curtainComponent);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var curtainComponent = await _curtainComponentService.GetByIdAsync(id);
            if (curtainComponent == null)
            {
                return NotFound();
            }

            var vm = new CurtainComponentEditViewModel
            {
                Id = curtainComponent.Id,
                Name = curtainComponent.Name,
                Cost = curtainComponent.Cost
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CurtainComponentEditViewModel vm)
        {
            // اعتبارسنجی سمت سرور
            if (!ModelState.IsValid)
                return View(vm);

            // نگاشت ViewModel -> Entity
            var curtainComponent = new CurtainComponent
            {
                Id = vm.Id,
                Name = vm.Name,
                Cost = vm.Cost,
            };

            // فراخوانی سرویس برای بروزرسانی
            var success = await _curtainComponentService.EditCurtainComponentAsync(curtainComponent);

            if (!success)
            {
                // اگر مشتری پیدا نشد یا خطایی رخ داد
                ModelState.AddModelError("", "خطایی در بروزرسانی اطلاعات مشتری رخ داد.");
                return View(vm);
            }

            TempData["Success"] = "اطلاعات مشتری با موفقیت بروزرسانی شد.";
            return RedirectToAction(nameof(Index)); // برگرد به لیست مشتری‌ها
        }
    }
}

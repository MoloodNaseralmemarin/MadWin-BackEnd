using MadWin.Application.Services;
using MadWin.Core.Entities.CurtainComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop2City.WebHost.ViewModels.CurtainComponents;
using System.Threading.Tasks;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
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
        public async Task<JsonResult> UpdateInline(int Id, string Name, string Cost)
        {
            try
            {
                var item =await _curtainComponentService.GetByIdAsync(Id);
                if (item == null)
                    return Json(new { success = false });

                item.Name = Name;
                item.Cost = decimal.Parse(Cost);
                item.LastUpdateDate = DateTime.Now;

                await _curtainComponentService.EditCurtainComponentAsync(item);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

    }
}

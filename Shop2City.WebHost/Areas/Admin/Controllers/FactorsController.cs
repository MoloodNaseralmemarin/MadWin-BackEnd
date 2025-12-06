using MadWin.Application.Services;
using MadWin.Core.DTOs.Factors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class FactorsController : Controller
    {
        private readonly IFactorDetailService _factorDetailService;

        public FactorsController(IFactorDetailService factorDetailService)
        {
            _factorDetailService = factorDetailService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(FactorFilterParameter filter, int pageId = 1)
        {
            var result = await _factorDetailService.GetAllFactorAsync(filter,pageId);
            return View(result);
        }


        public async Task<IActionResult> ShowFactorDetails(int id)
        {
            var getAllOrderDetails = await _factorDetailService.GetByFactorIdAsync(id);
            return PartialView("_FactorDetailsPartial", getAllOrderDetails);

        }
    }
}

using MadWin.Application.Services;
using MadWin.Core.DTOs.FilterParameters;
using MadWin.Core.DTOs.Orders;
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

        public FactorsController(IFactorDetailService factorDetailService)
        {
            _factorDetailService = factorDetailService;
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
    }
}

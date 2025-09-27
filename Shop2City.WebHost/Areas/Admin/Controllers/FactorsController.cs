using MadWin.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop2City.WebHost.ViewModels.Factors;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class FactorsController : Controller
    {
        private readonly IFactorService _factorService;

        public FactorsController(IFactorService factorService)
        {
            _factorService = factorService;
        }
        public async Task<IActionResult> GetAllFactors()
        {
            var allFactors = await _factorService.GetAllFactorsAsync();
            return View(allFactors);
        }
    }
}

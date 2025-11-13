using MadWin.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class SendSmsController : Controller
    {
        private readonly ISmsSenderService _smsSenderService;

        public SendSmsController(ISmsSenderService smsSenderService)
        {
            _smsSenderService = smsSenderService;
        }
        public async Task<IActionResult> Index(int pageId = 1)
        {
           var allsms = await _smsSenderService.GetAllSendSms(pageId);
            return View(allsms);
        }
    }
}

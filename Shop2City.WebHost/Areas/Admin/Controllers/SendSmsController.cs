using Microsoft.AspNetCore.Mvc;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    public class SendSmsController : Controller
    {
        public async Task<IActionResult> Index(int pageId = 1, string filterFirstName = "")
        {
           // var allUsers = await _userService.GetAllUsers(pageId, filterFirstName);
            return View();
        }
    }
}

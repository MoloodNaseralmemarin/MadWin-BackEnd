using Microsoft.AspNetCore.Mvc;
using Shop2City.Core.Security;

namespace Shop2City.WebHost.Controllers
{
    [PermissionChecker(1)]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

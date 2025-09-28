using MadWin.Application.Services;
using MadWin.Application.ViewModels.Account;
using MadWin.Core.Common;
using MadWin.Core.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shop2City.WebHost.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;

        public UsersController(IUserService userService, IPasswordHasher passwordHasher)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
        }
        public async Task<IActionResult> Index(int pageId = 1, string filterFirstName = "")
        {
            var allUsers = await _userService.GetAllUsers(pageId, filterFirstName);
            return View(allUsers);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (await _userService.IsExistCellPhoneAsync(model.CellPhone))
            {
                ModelState.AddModelError("CellPhone", ErrorMessage.InvalidCellPhone);
                return View(model);
            }
            model.Password = _passwordHasher.HashPassword(model.Password);
            var user = new User
            {
                Address = model.Address,
                CellPhone = model.CellPhone,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                UserName = model.CellPhone

            };
            await _userService.CreateUser(user);
            return RedirectToAction("Index");

        }
    }
}

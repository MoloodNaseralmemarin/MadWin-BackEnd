using MadWin.Application.Services;
using MadWin.Core.DTOs.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shop2City.WebHost.Pages.Admin.Users
{
   // [PermissionChecker(5)]
    public class DeleteModel : PageModel
    {
        private readonly IUserService _userService;
       
        public DeleteModel(IUserService userService)
        {
            _userService = userService;
        }

        public InformationUserViewModel informationUser { get; set; }
        public void OnGet(int id)
        {
            ViewData["UserId"] = id;
          //  informationUser = _userPanelService.GetInformationUser(id);
        }

        public IActionResult OnPost(int UserId)
        {
           // _userService.DeleteUser(UserId);
            return RedirectToPage("Index");
        }
    }
}
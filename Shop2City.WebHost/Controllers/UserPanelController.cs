using MadWin.Application.Services;
using MadWin.Core.Common;
using MadWin.Core.DTOs.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Shop2City.WebHost.Controllers
{
    public class UserPanelController : Controller
    {
        public readonly IUserPanelService _userPanelService;
        public readonly IUserService _userService;

        public UserPanelController(IUserPanelService userPanelService, IUserService userService)
        {
           _userPanelService = userPanelService;
            _userService = userService;
        }
        #region EditProfile
        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile()
        {
            return View(_userPanelService.GetDataForEditProfileUser(User.Identity.Name));
        }

        [HttpPost]
        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile(EditProfileViewModel editProfile)
        {
            if (!ModelState.IsValid)
                return View(editProfile);

            _userPanelService.EditProfile(User.Identity.Name, editProfile);
            #region Logout User
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            #endregion
            return Redirect("/Login?EditProfile=true");
        }
        #endregion
        #region ChangePassword
        [Route("UserPanel/ChangePassword")]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        [Route("UserPanel/ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordViewModel changePassword)
        {
            var currentUserName = User.Identity.Name;
            if (!ModelState.IsValid)
                return View(changePassword);

            //if (!_userService.CompareOldPassword(currentUserName, changePassword.currentPassword))
            //{
            //    ModelState.AddModelError(" currentPassword", ErrorMessage.CurrentPasswordIsIncorrect);
            //    return View(changePassword);
            //}
            //_userService.ChangeUserPassword(currentUserName, changePassword.password);
            ViewBag.IsSuccess = true;
            return View();
        }
        #endregion

    }
}

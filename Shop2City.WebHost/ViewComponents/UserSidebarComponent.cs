using MadWin.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Shop2City.WebHost.ViewComponents
{
    public class UserSidebarComponent : ViewComponent
    {
        private readonly IUserPanelService _userPanelService;
        public UserSidebarComponent(IUserPanelService userPanelService)
        {
            _userPanelService = userPanelService;
        }
        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            var user = await _userPanelService.GetSideBarUserPanelAsync(userId);
            return View(user);
        }
    }
}

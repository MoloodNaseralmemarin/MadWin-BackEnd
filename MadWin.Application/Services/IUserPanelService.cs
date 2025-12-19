using MadWin.Application.DTOs.Account;
using MadWin.Core.DTOs.Account;
using System.Collections.Generic;

namespace MadWin.Application.Services
{
    public interface IUserPanelService
    {

        Task<SideBarUserPanelDto> GetSideBarUserPanelAsync(int userId);

        EditProfileViewModel GetDataForEditProfileUser(string userName);

        void EditProfile(string userName, EditProfileViewModel editProfile);

        bool CompareOldPassword(string userName, string oldPassword);
        
    }
}

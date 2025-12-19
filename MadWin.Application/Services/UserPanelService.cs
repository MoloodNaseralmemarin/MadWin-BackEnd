using MadWin.Application.DTOs.Account;
using MadWin.Core.DTOs.Account;
using MadWin.Core.Interfaces;
using Shop2City.Core.Generator;


namespace MadWin.Application.Services
{
    public class UserPanelService : IUserPanelService
    {
        private readonly IUserRepository _userRepository;
        public UserPanelService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
 
        public async Task<SideBarUserPanelDto> GetSideBarUserPanelAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
               throw new Exception("کاربری یافت نشد.");

            var dto = new SideBarUserPanelDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                CreatedAt = user.CreatedAt,
                ImagePath = "~/images/useravatar/Defult.jpg",
            };

            return dto;
        }
        public EditProfileViewModel GetDataForEditProfileUser(string userName)
        {
            return null;// _context.Users
            //    .Where(u => u.UserName == userName)
            //    .Select(u => new EditProfileViewModel
            //    {
            //        telPhone = u.TelPhone,
            //        cellPhone = u.CellPhone,
            //        address = u.Address,
            //        userName = u.UserName
            //    }).Single();
        }

        public bool CompareOldPassword(string userName, string oldPassword)
        {
            return false;
            //string hashOldPassword = PasswordHelper.EncodePasswordMd5(oldPassword);
            //return _context.Users
            //    .Any(u => u.UserName == userName && u.Password == hashOldPassword);
        }

        void IUserPanelService.EditProfile(string userName, EditProfileViewModel editProfile)
        {
            throw new NotImplementedException();
        }
    }
}

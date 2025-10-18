using MadWin.Core.DTOs.Account;
using MadWin.Core.Interfaces;
using Shop2City.Core.Generator;


namespace MadWin.Application.Services
{
    public class UserPanelService : IUserPanelService
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        public UserPanelService(IUserService userServic, IUserRepository userRepository)
        {
            _userService = userServic;
            _userRepository = userRepository;
        }
        public async Task<InformationUserViewModel> GetInformationUser(string username)
        {
            var user =await _userService.GetUserByUserName(username);
            var informationUser = new InformationUserViewModel
            {
                userName = user.UserName,
                registerDate = user.CreateDate,
                fullName = user.FirstName + " " + user.LastName,
                cellPhone = user.CellPhone,
            };
            return informationUser;
        }

        public async Task<SideBarUserPanelViewModel> GetSideBarUserPanelDataAsync(int id)
        {
            var users = await _userRepository.GetAllAsync(); // فرضا Task<List<User>>

            var user = users
                .Where(u => u.Id == id)
                .Select(u => new SideBarUserPanelViewModel
                {
                    fullName = $"{u.FirstName} {u.LastName}",
                    registerDate = u.CreateDate
                })
                .SingleOrDefault();

            if (user == null)
            {
                return new SideBarUserPanelViewModel
                {
                    fullName = "کاربر یافت نشد",
                    registerDate = DateTime.MinValue
                };
            }

            return user;
        }



        public async Task EditProfile(string userName, EditProfileViewModel editProfile)
        {
            if (editProfile.userAvatarImageName != null)
            {
                var imagePath = "";

                #region DeleteImagePath
                if (editProfile.userAvatarImageName != "Defult.jpg")
                {
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/useravatar", editProfile.userAvatarImageName);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }

                }
                #endregion
                editProfile.userAvatarImageName = NameGenerator.GenerateUniqCode() + Path.GetExtension(editProfile.userAvatarFileName.FileName);
                #region saveImagePath
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/useravatar", editProfile.userAvatarImageName);
                using var stream = new FileStream(imagePath, FileMode.Create);
                editProfile.userAvatarFileName.CopyTo(stream);
                #endregion
            }
            var user =await _userService.GetUserByUserName(userName);
            user.TelPhone = editProfile.telPhone;
            user.CellPhone = editProfile.cellPhone;
            user.Address = editProfile.address;
            user.UserName = editProfile.userName;
            _userRepository.Update(user);
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

        public async Task ChangeUserPassword(string userName, string newPassword)
        {
          
            //var user =await _userService.GetUserByUserName(userName);
            //user.Password = PasswordHelper.EncodePasswordMd5(newPassword);
            //_userRepository.Update(user);
        }

        public bool CompareOldPassword(string userName, string oldPassword)
        {
            return false;
            //string hashOldPassword = PasswordHelper.EncodePasswordMd5(oldPassword);
            //return _context.Users
            //    .Any(u => u.UserName == userName && u.Password == hashOldPassword);
        }

        InformationUserViewModel IUserPanelService.GetInformationUser(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<InformationUserViewModel> GetInformationUser(int userId)
        {
            throw new NotImplementedException();
        }

        void IUserPanelService.EditProfile(string userName, EditProfileViewModel editProfile)
        {
            throw new NotImplementedException();
        }

        void IUserPanelService.ChangeUserPassword(string userName, string newPassword)
        {
            throw new NotImplementedException();
        }

        //public async Task<InformationUserViewModel> GetInformationUser(string username)
        //{
        //    var user =await _userService.GetUserByUserName(username);
        //    var informationUser = new InformationUserViewModel
        //    {
        //        userName = user.UserName,
        //        registerDate = user.CreateDate,
        //        fullName = user.FirstName + " " + user.LastName,
        //        cellPhone = user.CellPhone,

        //    };
        //    return informationUser;
        //}
    }
}

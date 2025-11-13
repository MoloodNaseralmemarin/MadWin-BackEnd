using MadWin.Application.DTOs.Account;
using MadWin.Core.DTOs.Users;
using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop2City.Core.Generator;
using System.Reflection;

namespace MadWin.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISmsSenderService _smsSenderService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, ISmsSenderService smsSenderService, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _smsSenderService = smsSenderService;
            _logger = logger;
        }

        public async Task<bool> IsExistCellPhoneAsync(string cellPhone)
        {
            return await _userRepository.IsExistCellPhoneAsync(cellPhone);
        }

        public async Task<bool> IsExistUserNameAsync(string userName)
        {
            return await _userRepository.IsExistUserNameAsync(userName);
        }

        public async Task<int> GetUserIdByUserName(string userName)
        {
            return await _userRepository.GetUserIdByUserName(userName);
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            return null;
        }

        public async Task<User> GetUserIdByUserId(int userId)
        {
            return await _userRepository.GetUserIdByUserId(userId);
        }
        public async Task<LoginResultDto?> LoginAsync(LoginResultDto loginDto)
        {
            var hashPassword = _passwordHasher.HashPassword(loginDto.Password);
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.UserName,hashPassword);

            if (user == null)
                return null;

            //بعدا درستش کن
            //var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);

            //if (verificationResult != PasswordVerificationResult.Success)
            //    return null;

            return new LoginResultDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = $"{user.FirstName} {user.LastName}"
            };
        }

        public async Task CreateUser(User user)
        {
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task ExtractAndSaveLast4DigitsAsync(string userName)
        {
            var user = await _userRepository.GetUserByUsernameAsync(userName);

            if (user != null && !string.IsNullOrWhiteSpace(user.CellPhone) && user.CellPhone.Length >= 4)
            {
                var newPassword = user.CellPhone.Substring(user.CellPhone.Length - 4);
                var hashPassword = _passwordHasher.HashPassword(newPassword);
                user.Password = hashPassword;
                user.Description = "کلمه عبور را تغییر داده است";
                await _userRepository.SaveChangesAsync();

                //ارسال پیامک به کاربر
                #region welcome SMS

                var smsSent = await _smsSenderService.SendSMSForgotPasswordForCustomer(user.CellPhone, newPassword);

                if (!smsSent)
                {
                    // فقط لاگ کن، بدون اطلاع دادن به کاربر
                    _logger.LogWarning("ارسال پیامک فراموشی کلمه عبور برای {Phone} موفق نبود. احتمالاً API پیامک خطا داده.", user.CellPhone);
                }

                #endregion
            }
        }

        public async Task<string> GetCellPhoneByUserIdAsync(int userId)
        {
            return await _userRepository.GetCellPhoneByUserIdAsync(userId);
        }

        public async Task<int> CountUserActive()
        {
            var usersIsActive = await _userRepository.GetAllAsync();
            return usersIsActive.Where(u => !u.IsDelete).Count();

        }

        public async Task<UserForAdminViewModel> GetAllUsers(int pageId = 1, string filterFirstName = "")
        {
            return await _userRepository.GetAllUsers(pageId, filterFirstName);
        }

        public async Task<EditUserViewModel> GetUserForEditAsync(int userId)
        {
            return await _userRepository.GetUserForShowEditModeAsync(userId);
        }

        public async Task EditUserFromAdmin(EditUserViewModel editUser)
        {
            var user = await _userRepository.GetByIdAsync(editUser.UserId);
            user.CellPhone = editUser.CellPhone;
            user.TelPhone = editUser.TelPhone;
            user.Address = editUser.Address;
            user.FirstName = editUser.FirstName;
            user.LastName = editUser.LastName;
            if (!string.IsNullOrEmpty(editUser.Password))
                editUser.Password = _passwordHasher.HashPassword(editUser.Password);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }
        public async Task DeleteUserFromAdmin(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            user.IsDelete = true;
            user.LastUpdateDate = DateTime.UtcNow;
            user.Description = "توسط آقای نادری حذف شده است.";
            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();
        }
        public async Task<UserNameDto?> GetNameByUserNameAsync(string userId)
        {
            return await _userRepository
                .GetQuery()
                .Where(u => u.Id.ToString() == userId)
                .Select(u => new UserNameDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName
                })
                .SingleOrDefaultAsync();
        }
    }
}

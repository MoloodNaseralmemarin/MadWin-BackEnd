using Ghasedak.Core.Interfaces;
using MadWin.Application.DTOs.Account;
using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Data;
using MadWin.Infrastructure.Repositories;
using MadWin.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace MadWin.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly MadWinDBContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISmsSenderService _smsSenderService;

        public AccountService(MadWinDBContext context, IPasswordHasher passwordHasher, ISmsSenderService smsSenderService, IAccountRepository accountRepository)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _smsSenderService = smsSenderService;
            _accountRepository = accountRepository;
        }
        public async Task CreateUserAsync(RegisterUserDto dto)
        {
            if (await _accountRepository.IsCellPhoneExistsAsync(dto.CellPhone))
                throw new Exception("این شماره قبلاً ثبت شده است.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CellPhone = dto.CellPhone,
                TelPhone=dto.TelPhone,
                Address = dto.Address,
                UserName =dto.Username,
                Password = _passwordHasher.Hash(dto.Password)
            };

            await _accountRepository.AddUserAsync(user);

            await _smsSenderService.SendWelcomeSmsAsync(
                dto.CellPhone,
                dto.FullName,
                2
            );
        }
    }
}

using MadWin.Application.DTOs.Account;

namespace MadWin.Application.Services
{
    public interface IAccountService
    {
        Task CreateUserAsync(RegisterUserDto dto);
    }
}

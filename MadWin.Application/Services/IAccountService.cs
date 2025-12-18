using MadWin.Application.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadWin.Application.Services
{
    public interface IAccountService
    {
        Task CreateUserAsync(RegisterUserDto dto);
    }
}

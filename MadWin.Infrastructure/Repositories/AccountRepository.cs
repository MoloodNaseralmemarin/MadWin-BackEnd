using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadWin.Infrastructure.Repositories
{
    public class AccountRepository : GenericRepository<User>, IAccountRepository
    {
        private readonly MadWinDBContext _context;

        public AccountRepository(MadWinDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsCellPhoneExistsAsync(string phone)
             => await _context.Users.AnyAsync(u => u.CellPhone == phone);
    }
}

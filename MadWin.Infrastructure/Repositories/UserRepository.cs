using MadWin.Core.DTOs.Users;
using MadWin.Core.Entities.SentMessages;
using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;
using MadWin.Core.Lookups.Account;
using MadWin.Core.Lookups.CommissionRates;
using MadWin.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly MadWinDBContext _context;

        public UserRepository(MadWinDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> GetUserIdByUserName(string userName)
        {
            var user = await _context.Users.SingleAsync(u => u.UserName == userName);
            return user.Id;
        }


        public async Task<bool> IsExistCellPhoneAsync(string cellPhone)
        {
            return await _context.Users.AnyAsync(u => u.CellPhone == cellPhone);
        }

        public async Task<bool> IsExistUserNameAsync(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName);
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            if (userName == null) return null;
            var getUserId =await _context.Users
                .SingleOrDefaultAsync(u => u.UserName == userName);
            return getUserId ?? null;

        }

        public async Task<User> GetUserByUserId(int userId)
        {
            return await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);
        }

        public Task<User> GetUserByUserId(string userName)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetUserIdByUserId(int userId)
        {
            return await _context.Users
                           .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username,string hashPassword)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username && u.Password==hashPassword);
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<UserInfoLookup> GetUserInfoByUserName(string userName)
        {
            var item = await _context.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (item == null)
                return null;

            return new UserInfoLookup
            {
                UserId = item.Id,
                UserName = item.UserName,
                Password=item.Password
            };
        }

        public async Task<string> GetCellPhoneByUserIdAsync(int userId)
        {
            var user = await _context.Users.SingleAsync(u => u.Id == userId);
            return user.CellPhone;
        }

        public async Task<UserForAdminViewModel> GetAllUsers(int pageId = 1, string filterFirstName = "")
        {
            IQueryable<User> result = GetQuery()
                .IgnoreQueryFilters()
                .Where(u => !u.IsDelete)
                .Include(u => u.Orders)
                .Include(u => u.Factors);

            if (!string.IsNullOrEmpty(filterFirstName))
            {
                result = result.Where(u => u.FirstName.Contains(filterFirstName));
            }

            int take = 10;
            int skip = (pageId - 1) * take;

            var list = new UserForAdminViewModel
            {
                currentPage = pageId,
                countPage = (int)Math.Ceiling(result.Count() / (double)take),
                Users = await result
                    .OrderByDescending(u => u.Id)
                    .Skip(skip)
                    .Take(take)
                    .Select(u => new UserForAdminItemViewModel
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        CellPhone=u.CellPhone,
                        CreateDate=u.CreateDate,
                        OrderCount = u.Orders.Count(),
                        FactorCount = u.Factors.Count()
                    })
                    .ToListAsync()
            };

            return list;
        }
        public async Task<EditUserViewModel> GetUserForShowEditModeAsync(int userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new EditUserViewModel
                {
                    CellPhone = u.CellPhone,
                    TelPhone = u.TelPhone,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserId = u.Id,
                    Address = u.Address
                })
                .SingleOrDefaultAsync();
        }
    }
}

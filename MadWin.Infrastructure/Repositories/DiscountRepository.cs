using MadWin.Core.DTOs.DisCounts;
using MadWin.Core.DTOs.Users;
using MadWin.Core.Entities.Discounts;
using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;
using MadWin.Core.Lookups.Discounts;
using MadWin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MadWin.Infrastructure.Repositories
{
    public class DiscountRepository : GenericRepository<Discount>, IDiscountRepository
    {
        private readonly MadWinDBContext _context;
        public DiscountRepository(MadWinDBContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Discount> GetDiscountForOrderInfoAsync(string code)
        {
            return await _context.Set<Discount>()
                .FirstOrDefaultAsync(dc => dc.DiscountCode == code && dc.Item == "O");
        }

        public async Task<DiscountInfoLookup> GetDiscountInfoByDiscountIdAsync(int discountId)
        {
            var discount = await _context.Set<Discount>()
                .AsNoTracking()
                .Where(dc => dc.Id == discountId)
                .Select(dc => new DiscountInfoLookup
                {
                    DiscountCode = dc.DiscountCode,
                    DiscountId = dc.Id,
                    DiscountPercent = dc.Percentage,
                })
                .FirstOrDefaultAsync();
            if (discount == null)
                return null;
            return discount;
        }

        public async Task<Discount> GetDiscountForFactorInfoAsync(string code)
        {
            return await _context.Set<Discount>()
                .FirstOrDefaultAsync(dc => dc.DiscountCode == code && dc.Item == "F");
        }


        public async Task<bool> HasUserUsedDiscountForOrderAsync(int userId, int discountId, int orderId)
        {
            return await _context.Set<UserDiscountCode>()
                .AnyAsync(x => x.UserId == userId
                            && x.DisCountId == discountId
                            && x.OrderId == orderId);
        }

        public async Task<bool> HasUserUsedDiscountForFactorAsync(int userId, int discountId, int factorId)
        {
            return await _context.Set<UserDiscountCode>()
                .AnyAsync(x => x.UserId == userId
                            && x.DisCountId == discountId
                            && x.FactorId == factorId);
        }

        public async Task<bool> IsExistDisCountCode(string discountCode)
        {
            var result = await GetByConditionAsync(dc => dc.DiscountCode == discountCode);
            return result.Any();
        }
        public async Task<DiscountForAdminViewModel> GetAllDiscountsAsync(int pageId)
        {
            IQueryable<Discount> result = GetQuery()
                          .IgnoreQueryFilters()
                          .Where(u => !u.IsDelete);

            int take = 10;
            int skip = (pageId - 1) * take;

            var list = new DiscountForAdminViewModel
            {
                CurrentPage = pageId,
                CountPage = (int)Math.Ceiling(result.Count() / (double)take),
                Discounts = await result
                    .OrderByDescending(u => u.Id)
                    .Skip(skip)
                    .Take(take)
                    .Select(d => new DiscountForAdminItemViewModel
                    {
                        Id = d.Id,
                        CreateDate = d.UpdatedAt,
                        DiscountCode = d.DiscountCode,
                        ExpiryDate = d.ExpiryDate,
                        Item = d.Item,
                        Percentage = d.Percentage,
                        StartDate = d.StartDate,
                        InitialCount=d.InitialCount,
                        UseableCount = d.UseableCount,
                        Description = d.Description


                    })
                    .ToListAsync()
            };

            return list;
        }
    }
}
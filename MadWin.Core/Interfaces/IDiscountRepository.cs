
using MadWin.Core.DTOs.DisCounts;
using MadWin.Core.Entities.Discounts;
using MadWin.Core.Lookups.Discounts;

namespace MadWin.Core.Interfaces
{
    public interface IDiscountRepository:IGenericRepository<Discount>
    {
        Task<Discount> GetDiscountForOrderInfoAsync(string code);
        Task<Discount> GetDiscountForFactorInfoAsync(string code);

        Task<DiscountInfoLookup> GetDiscountInfoByDiscountIdAsync(int discountId);

        Task<bool> HasUserUsedDiscountForOrderAsync(int userId, int discountId, int orderId);

        Task<bool> HasUserUsedDiscountForFactorAsync(int userId, int discountId, int factorId);

        Task<bool> IsExistDisCountCode(string discountcode);
        Task<DiscountForAdminViewModel> GetAllDiscountsAsync(int pageId = 1);

        
    }
}

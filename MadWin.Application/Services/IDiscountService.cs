using MadWin.Application.DTOs.Discounts;
using MadWin.Core.DTOs.DisCounts;
using MadWin.Core.DTOs.Orders;
using MadWin.Core.Entities.Discounts;
using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;

namespace MadWin.Application.Services
{
    public interface IDiscountService
    {
        Task<DiscountResultDto> ApplyDiscountForOrderAsync(int orderId, string discountCode);

        Task<DiscountResultDto> ApplyDiscountForFactorAsync(int factorId, string discountCode);
        Task<DiscountUseType> UseDiscountForFactorAsync(int factorId, string code, int userId);
        Task<DiscountUseType> UseDiscountForOrderAsync(int orderId, string code, int userId);


        Task<Discount> GetByIdAsync(int id);
        Task<DiscountForAdminViewModel> GetAllDiscountsAsync(int pageId = 1);

        Task AddDiscount(Discount discount);

        Task<bool> IsExistDisCountCode(string discountCode);

    }
}
        
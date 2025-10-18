using MadWin.Application.DTOs.Discounts;
using MadWin.Core.DTOs.DisCounts;
using MadWin.Core.DTOs.Orders;
using MadWin.Core.Entities.Discounts;
using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;

namespace MadWin.Application.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IUserDiscountCodeRepository _userDiscountCodeRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IFactorRepository _factorRepository;

        public DiscountService(IDiscountRepository discountRepository,IUserDiscountCodeRepository userDiscountCodeRepository, IOrderRepository orderRepository, IFactorRepository factorRepository)
        {
            _discountRepository = discountRepository;
            _userDiscountCodeRepository = userDiscountCodeRepository;
            _orderRepository = orderRepository;
            _factorRepository = factorRepository;
        }


        public async Task<DiscountResultDto> ApplyDiscountForOrderAsync(int orderId, string discountCode)

        {
            var result = new DiscountResultDto();

            //var order = await _orderRepository.GetOrderInfoByOrderIdAsync(orderId);
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return null;
            }

            //ببین میتونی در اینده بکنیتش ی سروریس
            var discount = await _discountRepository.GetDiscountForOrderInfoAsync(discountCode);
            result.OriginalPrice = order.PriceWithFee;
            result.DiscountPercentage = discount.Percentage;

            result.DiscountAmount = (result.OriginalPrice * discount.Percentage) / 100;
            result.FinalPrice = order.PriceWithFee - result.DiscountAmount;

            #region update discount
            order.DiscountId = discount.Id;
            order.DisPercent = discount.Percentage;
            order.DisTotal = result.DiscountAmount;
            await _orderRepository.SaveChangesAsync();
            #endregion

            return result;
        }
        public async Task<DiscountResultDto> ApplyDiscountForFactorAsync(int factorId, string discountCode)
        {
            var result = new DiscountResultDto();

            var factor = await _factorRepository.GetFactorInfoByFactorIdAsync(factorId);
            if (factor == null)
            {
                return null;
            }

            var discount = await _discountRepository.GetDiscountForFactorInfoAsync(discountCode);
            result.OriginalPrice = factor.Price;
            result.DiscountPercentage = discount.Percentage;

            result.DiscountAmount = (result.OriginalPrice * discount.Percentage) / 100;
            result.FinalPrice = factor.Price - result.DiscountAmount;

            return result;
        }

        public async Task<DiscountUseType> UseDiscountForOrderAsync(int orderId, string code, int userId)

        {
            var discount = await _discountRepository.GetDiscountForOrderInfoAsync(code);

            if (discount == null)
                return DiscountUseType.NotFound;

            if (discount.ExpiryDate <= DateTime.Now)
                return DiscountUseType.ExpirationDate;

            if (discount.UseableCount < 1)
                return DiscountUseType.Finished;

            if (await _discountRepository.HasUserUsedDiscountForOrderAsync(userId, discount.Id, orderId))
                return DiscountUseType.UserUsed;

            await _userDiscountCodeRepository.AddAsync(new UserDiscountCode
            {
                UserId = userId,
                DisCountId = discount.Id,
                OrderId = orderId
            });
            await _userDiscountCodeRepository.SaveChangesAsync();
            return DiscountUseType.Success;
        }

        public async Task<DiscountUseType> UseDiscountForFactorAsync(int factorId, string code, int userId)
        {
            var discount = await _discountRepository.GetDiscountForFactorInfoAsync(code);

            if (discount == null)
                return DiscountUseType.NotFound;

            if (discount.ExpiryDate <= DateTime.Now)
                return DiscountUseType.ExpirationDate;

            if (discount.UseableCount < 1)
                return DiscountUseType.Finished;

            if (await _discountRepository.HasUserUsedDiscountForFactorAsync(userId, discount.Id, factorId))
                return DiscountUseType.UserUsed;

            await _userDiscountCodeRepository.AddAsync(new UserDiscountCode
            {
                UserId = userId,
                DisCountId = discount.Id,
                FactorId = factorId
            });
            if (discount.UseableCount != null)
            {
                discount.UseableCount -= 1;
            }
         
            await _discountRepository.SaveChangesAsync();
            return DiscountUseType.Success;
        }

        public async Task<DiscountForAdminViewModel> GetAllDiscountsAsync(int pageId = 1)
        {
            return await _discountRepository.GetAllDiscountsAsync(pageId);
        }

        public async Task<Discount> GetByIdAsync(int id)
        {
            return await _discountRepository.GetByIdAsync(id);
        }

        public async Task AddDiscount(Discount discount)
        {
            await _discountRepository.AddAsync(discount);
            await _discountRepository.SaveChangesAsync();
        }
        public async Task<bool> IsExistDisCountCode(string discountCode)
        {
            return await _discountRepository.IsExistDisCountCode(discountCode);
        }
    }
}

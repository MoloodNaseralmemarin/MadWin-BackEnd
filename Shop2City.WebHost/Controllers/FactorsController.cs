
using MadWin.Application.Services;
using MadWin.Core.DTOs.Orders;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Shop2City.WebHost.Controllers
{
    public class FactorsController : Controller
    {
        private readonly IFactorDetailService _factorDetailService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IDiscountService _discountService;
        private readonly IFactorService _factorService;
        public FactorsController(IFactorDetailService factorDetailService, IDeliveryMethodService deliveryMethodService, IDiscountService discountService, IFactorService factorService)
        {
            _factorDetailService = factorDetailService;
            _deliveryMethodService = deliveryMethodService;
            _discountService = discountService;
            _factorService = factorService;
        }

        #region حذف آیتم از فاکتور

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItemsByFactorAsync(int[] factorId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();
            await _factorDetailService.SoftDeleteAsync(factorId);
            var updatedList = await _factorService.GetFactorSummaryByUserIdAsync(userId);
            return Json(new { success = true, data = updatedList });
        }
        #endregion




        [HttpGet]
        public IActionResult GetSubtotalPrice(int factorId)
        {
            var subtotal = _factorService.GetSubtotal(factorId);
            return Json(new { price = subtotal.ToString("N0") });

        }
        #region کد تخفیف
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UseDiscountForFactorAsync(int factorId, string discountCode)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Json(new { success = false, message = "کاربر نامعتبر" });


            if (string.IsNullOrEmpty(discountCode))
                return Json(new { success = false, message = "کد تخفیف وارد نشده است." });


            var result = await _discountService.UseDiscountForFactorAsync(factorId, discountCode, userId);


            switch (result)
            {
                case DiscountUseType.Success:

                    var applyDiscount = await _discountService.ApplyDiscountForFactorAsync(factorId, discountCode);
                    return Json(new
                    {
                        success = true,
                        discountAmount = applyDiscount.DiscountAmount,
                        message = "کد تخفیف با موفقیت اعمال شد"
                    });

                case DiscountUseType.ExpirationDate:
                    return Json(new { success = false, message = "مهلت استفاده از کد تخفیف به پایان رسیده است" });

                case DiscountUseType.NotFound:
                    return Json(new { success = false, message = "کد تخفیف یافت نشد" });

                case DiscountUseType.Finished:
                    return Json(new { success = false, message = "سقف استفاده از این کد تخفیف به پایان رسیده است" });

                case DiscountUseType.UserUsed:
                    return Json(new { success = false, message = "شما قبلاً از این کد تخفیف استفاده کرده‌اید" });

                default:
                    return Json(new { success = false, message = "خطای ناشناخته‌ای رخ داده است" });
            }
        }
        #endregion



    }
}


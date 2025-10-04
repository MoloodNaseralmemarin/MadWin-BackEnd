using MadWin.Application.DTOs.Cart;
using MadWin.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Shop2City.WebHost.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IServiceProvider _serviceProvider;

        public ProductsController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        [Authorize]
        public async Task<IActionResult> BuyProduct()
        {
            // دریافت UserId از claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(); // اگر کاربر لاگین نکرده باشد
            }

            // دریافت سبد خرید از session
            var cart = HttpContext.Session.GetObjectFromJson<List<ShopCartitemDto>>("Cart") ?? new List<ShopCartitemDto>();

            // اگر سبد خرید خالی باشد
            if (!cart.Any())
            {
                return RedirectToAction("Index", "Home"); // کاربر را به صفحه اصلی هدایت کن
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var factorService = scope.ServiceProvider.GetRequiredService<IFactorService>();

                // فقط یکبار فاکتور رو بساز یا بیار
                var factorId = await factorService.CreateOrGetOpenFactorAsync(userId);

                // حالا همه‌ی آیتم‌ها رو اضافه کن
                foreach (var item in cart)
                {
                    await factorService.AddOrUpdateFactorDetailAsync(factorId, item.ProductId, item.Count);
                }

                // آپدیت مجموع فاکتور
                await factorService.UpdateFactorSumAsync(factorId);

                // هدایت به صفحه خلاصه فاکتور
                return RedirectToAction("FactorSummary", "Factors", new { area = "UserPanel", factorId = factorId });

            }
        }
    }
}
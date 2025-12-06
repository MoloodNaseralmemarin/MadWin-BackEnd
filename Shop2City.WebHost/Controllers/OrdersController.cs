
using MadWin.Application.Services;
using MadWin.Core.DTOs.Orders;
using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;
using MadWin.Core.Lookups.CommissionRates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop2City.Core.Services.Products;
using Shop2City.WebHost.ViewModels.Orders;
using System.Security.Claims;

namespace Shop2City.Web.Areas.UserPanel.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICurtainComponentDetailService _curtainComponentDetailService;

        private readonly IDiscountService _discountService;
        private readonly ICommissionRateRepository _commissionRateRepository;
        private readonly ICurtainComponentRepository _curtainComponentRepository;

        public OrdersController(
            IProductService productService,
            IOrderService orderService,
            ICurtainComponentDetailService curtainComponentDetailService,

            IDiscountService discountService,
            ICommissionRateRepository commissionRateRepository,
            ICurtainComponentRepository curtainComponentRepository)
        {
            // guard clauses - throw early if DI failed so NullReferenceExceptions are easier to trace
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _curtainComponentDetailService = curtainComponentDetailService ?? throw new ArgumentNullException(nameof(curtainComponentDetailService));

            _discountService = discountService ?? throw new ArgumentNullException(nameof(discountService));
            _commissionRateRepository = commissionRateRepository ?? throw new ArgumentNullException(nameof(commissionRateRepository));
            _curtainComponentRepository = curtainComponentRepository ?? throw new ArgumentNullException(nameof(curtainComponentRepository));
        }
        public async Task<IActionResult> CreateOrder()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();
            // گرفتن دسته‌ها
            var categories = await _productService.GetCategoryForManageProduct(1);

            // تعریف subCategories خالی برای استفاده بعدی
            List<SelectListItem> subCategories = new List<SelectListItem>();

            // گرفتن زیر دسته‌ها بر اساس دسته اول
            var firstCategoryId = categories.FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(firstCategoryId) && int.TryParse(firstCategoryId, out int categoryId))
            {
                subCategories = await _productService.GetSubCategoryForManageProduct(categoryId);
            }

            var model = new OrderViewModel
            {
                UserId= userId,
                Categories = new SelectList(categories, "Value", "Text"),
                SubCategories = new SelectList(subCategories, "Value", "Text")
            };

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> GetSubCategories(int categoryId)
        {
            var items =await _productService.GetSubCategoryForManageProduct(categoryId);

            // باید خروجی JSON به شکلی باشه که جاوااسکریپت بتونه بخونه
            return Json(items.Select(s => new {
                Id = s.Value,
                Name = s.Text
            }));
        }

        #region ثبت سفارش 
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateDto orderView)
        {
            // اگر ورودی مدل نامعتبر است، برای فراخوانی از مودال/آژاکس بهتر است خطاها را برگردانیم
            if (!ModelState.IsValid)
            {
                // در حالت آژاکس، خطاها را به صورت json ارسال کن
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(ModelState);

                // در حالت معمولی، دوباره ویو را با دیتاهای لازم بازگردان
         
                return View(orderView);
            }

            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                    return Unauthorized();

                // حداقل مقدارها
                var height = Math.Max(orderView.Height, 200);
                var width = Math.Max(orderView.Width, 80);

                decimal basePrice = 0m;

                var items = await _orderService.GetCalculationAsync(orderView.CategoryId, orderView.SubCategoryId);

                if (items == null || !items.Any())
                {
                    // اگر این متد داخل مودال فراخوانی شده، بهتر است یک PartialView یا Json برگردانیم
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "هیچ مولفه‌ای برای محاسبه یافت نشد." });

                    
                    ModelState.AddModelError(string.Empty, "هیچ مولفه‌ای برای محاسبه یافت نشد.");
                    return View(orderView);
                }

                // ثبت سفارش اولیه و گرفتن orderId
                int orderId = await _orderService.CreateOrderInitialAsync(orderView, userId, 0);

                // محاسبه قیمت‌ها و ذخیره جزئیات اجزا
                foreach (var item in items)
                {
                    decimal componentCost = item.CurtainComponentId switch
                    {
                        1 => await CalculateIraniAsync(orderView),
                        2 => await CalculateKharejiAsync(orderView),
                        3 => await CalculateTooriOneLayerAsync(orderView),
                        4 => await CalculateTooriTwoLayerAsync(orderView, orderView.PartCount),
                        5 => await CalculateZipper5CostAsync(orderView.Width),

                        // اگر چسب نبود، زیپ ۲ حساب شود
                        6 => !orderView.IsCurtainAdhesive
                                ? await CalculateZipper2CostAsync(orderView.Height)
                                : 0m,

                        7 => await CalculateChodonCostAsync(orderView.Width),
                        8 => await CalculateGanCostAsync(orderView.Height, orderView.PartCount),
                        9 => await CalculateMagnetCostAsync(orderView.Height, orderView.PartCount),
                        10 => await CalculateGlue4CostAsync(orderView.Width),
                        11 => await CalculateGlue2CostAsync(orderView.Height),
                        12 => await GetWageCostAsync(),
                        13 => await GetPackagingCostAsync(),

                        14 => await CalculateIrani50Async(orderView),
                        15 => await CalculateKhareji50Async(orderView),

                        // اگر چسب فعال بود، هزینه چسب ۴ لایه حساب شود
                        16 => orderView.IsCurtainAdhesive
                                ? await CalculateCurtainAdhesiveAsync(orderView.Height)
                                : 0m,

                        _ => 0m
                    };

                    basePrice += componentCost;

                    // لاگ ساده برای دیباگ (در تولید از ILogger استفاده کنید)
                    Console.WriteLine($"Saving component: {item.CurtainComponentId} with cost {componentCost}");

                    await _curtainComponentDetailService.CreateCurtainComponentDetailInitialAsync(orderId, item.CurtainComponentId, componentCost, orderView.Count);
                }

                // محاسبه کارمزد
                var commission = await GetCommissionInfoAsync(orderView.PartCount, orderView.IsEqualParts);

                if (commission == null)
                {
                    // اگر کارمزد پیدا نشد، آپدیت را انجام نده و پیغام مناسب بازگردان
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "اطلاعات کارمزد یافت نشد." });

                    ModelState.AddModelError(string.Empty, "اطلاعات کارمزد یافت نشد.");
                    
                    return View(orderView);
                }

                // آپدیت قیمت و کارمزد
                await _orderService.UpdatePriceAndCommissionAsync(orderId, basePrice, commission.CommissionPercent, commission.CommissionRateId);

                // برای نمایش در مودال یا نتیجه‌ی آژاکس، یک پاسخ مناسب بازگردان
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // در اینجا می‌توان اطلاعات خلاصه سفارش را برگرداند
                    var summary = await _orderService.GetTodayOrdersAsync(userId);
                    return Json(new { success = true, orderId, summary });
                }

                // در حالت غیر آژاکس، ریدایرکت یا بازگشت به صفحه‌ی مناسب
                return RedirectToAction(nameof(CreateOrder));
            }
            catch (Exception ex)
            {
                // در محیط تولید از ILogger استفاده کنید؛ برای دیباگ فعلا لاگ کن
                Console.WriteLine(ex);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return StatusCode(500, new { success = false, message = "خطای سرور. با پشتیبانی تماس بگیرید." });

                ModelState.AddModelError(string.Empty, "خطای سرور. با پشتیبانی تماس بگیرید.");
                
                return View(orderView);
            }
        }
        #endregion

        #region پرده طلقی ایرانی
        private async Task<decimal> CalculateIraniAsync(CreateDto order)
        {
            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(1);
            if (unitPrice <= 0) return 0;
            return ((order.Height + 10) * order.Width * unitPrice) / 10000;
        }
        #endregion

        #region پرده طلقی خارجی
        private async Task<decimal> CalculateKharejiAsync(CreateDto order)
        {
            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(2);
            if (unitPrice <= 0) return 0;
            return ((order.Height + 10) * order.Width * unitPrice) / 10000;
        }
        #endregion
        //تغییرات 1404-08-26
        #region پرده طلقی رویال ایرانی  (ضخامت 0.50)
        private async Task<decimal> CalculateIrani50Async(CreateDto order)
        {
            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(14);
            if (unitPrice <= 0) return 0;
            return ((order.Height + 10) * order.Width * unitPrice) / 10000;
        }
        #endregion
        //تغییرات 1404-08-26
        #region پرده طلقی رویال خارجی  (ضخامت 0.50)
        private async Task<decimal> CalculateKhareji50Async(CreateDto order)
        {
            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(15);
            if (unitPrice <= 0) return 0;
            return ((order.Height + 10) * order.Width * unitPrice) / 10000;
        }
        #endregion

        #region پرده توری یک لایه
        private async Task<decimal> CalculateTooriOneLayerAsync(CreateDto order)
        {
            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(3);
            if (unitPrice <= 0) return 0;
            return ((order.Height + 10) * order.Width * unitPrice) / 10000;
        }
        #endregion

        #region پرده توری دو لایه
        private async Task<decimal> CalculateTooriTwoLayerAsync(CreateDto order, int partCount)
        {
            decimal totalCost = 0;

            decimal unitPriceSecondLayer = await _curtainComponentRepository.GetPriceByIdAsync(4);
            if (unitPriceSecondLayer <= 0) return 0;

            decimal heightPlusMargin = order.Height + 10;
            decimal twoLayerArea = heightPlusMargin * 40;
            decimal twoLayerCost = (twoLayerArea / 10000) * unitPriceSecondLayer;

            decimal singleLayerCostPerMeter = await _curtainComponentRepository.GetPriceByIdAsync(3);
            decimal singleLayerCost = (heightPlusMargin * order.Width * singleLayerCostPerMeter) / 10000;
            totalCost += singleLayerCost + twoLayerCost;

            // سایر اجزا
            totalCost += await CalculateZipper5CostAsync(order.Width);
            // اگر چسب نبود، زیپ ۲ حساب شود
            totalCost += !order.IsCurtainAdhesive
                    ? await CalculateZipper2CostAsync(order.Height)
                    : 0m;
            totalCost += await CalculateZipper2CostAsync(order.Height);
            totalCost += await CalculateChodonCostAsync(order.Width);
            totalCost += await CalculateGanCostAsync(order.Height, order.PartCount);
            totalCost += await CalculateMagnetCostAsync(order.Height, order.PartCount);
            totalCost += await CalculateGlue4CostAsync(order.Width);
            totalCost += await CalculateGlue2CostAsync(order.Height);
            totalCost += await GetWageCostAsync();
            totalCost += await GetPackagingCostAsync();
            totalCost += order.IsCurtainAdhesive
                   ? await CalculateCurtainAdhesiveAsync(order.Height)
                   : 0m;

            var isTriplePart = partCount == 3;
            decimal a = totalCost + twoLayerCost;
            return isTriplePart ? a : totalCost;
        }
        #endregion

        #region زیپ چسب 5 سانت
        private async Task<decimal> CalculateZipper5CostAsync(int width)
        {
            const decimal coefficient = 0.01M;
            const int extraWidth = 5;

            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(5);
            if (unitPrice <= 0) return 0;

            return (((width + extraWidth) * coefficient) * unitPrice);
        }
        #endregion

        #region زیپ چسب 2.5 سانت
        private async Task<decimal> CalculateZipper2CostAsync(int height)
        {
            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(6);
            if (unitPrice <= 0) return 0;
            var adjustedHeight = GetAdjustedHeight(height);
            decimal a = (decimal)adjustedHeight / 100;
            decimal b = (decimal)a * unitPrice;
            return b;
        }
        #endregion

        #region جودون
        private async Task<decimal> CalculateChodonCostAsync(int width)
        {
            const decimal coefficient = 0.01M;
            const int extraWidth = 2;

            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(7);
            if (unitPrice <= 0) return 0;
            var adjustedWidth = width + extraWidth;

            decimal resultChodon = adjustedWidth * coefficient * unitPrice;

            return resultChodon;
        }
        #endregion

        #region گان
        private async Task<decimal> CalculateGanCostAsync(int height, int partCount)
        {
            const decimal coefficient = 0.01M;
            const int extraHeight = 10;
            const decimal widthFactor = 4.2M;  //وزن هر متر گان     
            const int quantity = 4;

            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(8);

            if (unitPrice <= 0) return 0;

            int adjustedHeight = height + extraHeight;
            decimal resultGan = adjustedHeight * coefficient * widthFactor * quantity * unitPrice;

            var newGan = resultGan / 2;

            var isTriplePart = partCount == 3;
            decimal a = resultGan + newGan;
            return isTriplePart ? a : resultGan;
        }
        #endregion

        #region آهنربا
        private async Task<decimal> CalculateMagnetCostAsync(int height, int partCount)
        {
            const decimal magnetSpacing = 13.5M;
            const int threshold1 = 200;
            const int threshold2 = 400;

            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(9);
            if (unitPrice <= 0) return 0;

            decimal result = 0;

            if (height >= 0 && height <= threshold1)
            {
                int effectiveHeight = height - 20;
                if (effectiveHeight > 0)
                {
                    int magnetCount = (int)Math.Round(effectiveHeight / magnetSpacing) * 2;
                    result = magnetCount * unitPrice;
                }
            }
            else if (height > threshold1 && height <= threshold2)
            {
                int effectiveHeight = height - 30;
                if (effectiveHeight > 0)
                {
                    int magnetCount = (int)Math.Ceiling(effectiveHeight / magnetSpacing) * 2;
                    result = magnetCount * unitPrice;
                }
            }
            else
            {
                Console.WriteLine("خطا: ارتفاع خارج از محدوده مجاز است.");
            }
            var isTriplePart = partCount == 3;
            return isTriplePart ? result * 2 : result;
        }
        #endregion

        #region چسب 2 طرفه 4 سانت
        private async Task<decimal> CalculateGlue4CostAsync(int width)
        {
            const decimal coefficient = 0.01M;
            const int extraWidth = 5;

            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(10);
            if (unitPrice <= 0) return 0;

            var adjustedwidth = width + extraWidth;
            decimal resultGlue4 = adjustedwidth * coefficient * unitPrice;

            return resultGlue4;
        }
        #endregion

        #region چسب 2 طرفه 2 سانت
        private async Task<decimal> CalculateGlue2CostAsync(int height)
        {
            const decimal coefficient = 100;

            var adjustedHeight = GetAdjustedHeight(height);

            var unitPrice = await _curtainComponentRepository.GetPriceByIdAsync(11);
            decimal resultGlue2 = adjustedHeight / coefficient * unitPrice;

            return resultGlue2;
        }
        #endregion

        #region اجرت دوخت
        public async Task<decimal> GetWageCostAsync()
        {
            var cost = await _curtainComponentRepository.GetPriceByIdAsync(12);
            return cost;
        }
        #endregion

        #region اجرت بسته بندی
        public async Task<decimal> GetPackagingCostAsync()
        {
            var cost = await _curtainComponentRepository.GetPriceByIdAsync(13);

            return cost;
        }
        #endregion

        #region محاسبه ارتفاع برای زیپ چسب 2.5 سانت و گان
        public int GetAdjustedHeight(int height)
        {
            int heightNew = 0;
            switch (height)
            {
                case int n when n >= 0 && n <= 230:
                    heightNew = 90;
                    break;
                case int n when n >= 231 && n <= 270:
                    heightNew = 110;
                    break;
                case int n when n >= 271 && n <= 300:
                    heightNew = 125;
                    break;
                case int n when n >= 301 && n <= 330:
                    heightNew = 145;
                    break;
                case int n when n >= 331 && n <= 360:
                    heightNew = 165;
                    break;
                case int n when n >= 361 && n <= 400:
                    heightNew = 185;
                    break;
                default:
                    Console.WriteLine("error");
                    break;
            }
            return heightNew;
        }
        #endregion

        #region محاسبه کارمزد
        private async Task<CommissionInfoLookup> GetCommissionInfoAsync(int partCount, bool isEqualParts)
        {
            return await _commissionRateRepository.GetCommissionInfoAsync(partCount, isEqualParts);
        }
        #endregion


        #region چسب بغل پرده
        public async Task<decimal> CalculateCurtainAdhesiveAsync(int height)
        {
            // اینو آقای نادری تاییدش کرد
            //1404-08-28
            const decimal coefficient = 0.01M;
            const int extraWidth = 2;
            var cost = await _curtainComponentRepository.GetPriceByIdAsync(16);
            var result = ((height * coefficient) * extraWidth) * cost;
            return result;
        }
        #endregion

        #region کد تخفیف
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UseDiscountForOrderAsync(int orderId, string discountCode)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Json(new { success = false, message = "کاربر نامعتبر" });


            if (string.IsNullOrEmpty(discountCode))
                return Json(new { success = false, message = "کد تخفیف وارد نشده است." });


            var result = await _discountService.UseDiscountForOrderAsync(orderId, discountCode, userId);


            switch (result)
            {
                case DiscountUseType.Success:

                    var applyDiscount = await _discountService.ApplyDiscountForOrderAsync(orderId, discountCode);
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
        #region حذف از سفارش
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItemsByOrderAsync(int[] orderId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();
            await _orderService.SoftDeleteFromOrderAsync(orderId);
            var updatedList = await _orderService.GetTodayOrdersAsync(userId);
            return Json(new { success = true, data = updatedList });
        }
        #endregion

        #region جمع کل سفارش روز +‌کاربر
        [HttpGet]
        public IActionResult GetSumPriceWithFeeByOrder(int[] orderId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();
            var subtotal = _orderService.GetSumPriceWithFeeByOrder(orderId, userId);
            return Json(new { price = subtotal });

        }
        #endregion


    }
}

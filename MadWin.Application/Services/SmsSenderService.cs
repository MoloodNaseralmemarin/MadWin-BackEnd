using Ghasedak.Core;
using MadWin.Core.DTOs.Users;
using MadWin.Core.Entities.SentMessages;
using MadWin.Core.Interfaces;
using MadWin.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MadWin.Application.Services
{
    public class SmsSenderService : ISmsSenderService
    {
        private readonly ISmsRepository _smsRepository;
        private readonly ILogger<SmsSenderService> _logger;
        private readonly SmsSettings _smsSettings;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderWidthPartRepository _orderWidthPartRepository;
        private readonly IFactorDetailService _factorDetailService;
        private readonly IFactorService _factorService;

        public SmsSenderService(
            ISmsRepository smsRepository,
            ILogger<SmsSenderService> logger,
            IOptions<SmsSettings> smsSettings,
            IOrderRepository orderRepository,
            IOrderWidthPartRepository orderWidthPartRepository,
            IFactorDetailService factorDetailService,
            IFactorService factorService)
        {
            _smsRepository = smsRepository;
            _logger = logger;
            _smsSettings = smsSettings.Value;
            _orderRepository = orderRepository;
            _orderWidthPartRepository = orderWidthPartRepository;
            _factorDetailService = factorDetailService;
            _factorService = factorService;
        }
        public async Task<bool> SendWelcomeSmsAsync(string cellPhone, string fullName, int smsCount)
        {
            var message = $"آقا/خانم {fullName} به فروشگاه آنلاین ماد وین خوش آمدید.";
            try
            {
                var api = new Api(_smsSettings.ApiKey);
                var result = await api.VerifyAsync(1, "WelComeMadWin", new[] { cellPhone }, fullName, "ماد وین");

                if (result != null)
                {
                    await SaveSmsAsync(cellPhone, "WelComeMadWin", message, "کاربر جدید", 2, "200", null, null);
                    _logger.LogInformation("پیامک خوش‌آمدگویی با موفقیت ارسال شد.");
                    return true;
                }
                else
                {
                    _logger.LogWarning("ارسال پیامک خوش‌آمدگویی با خطا مواجه شد. نتیجه خالی بود.");
                    return false;
                }
            }
            catch(Exception ex)
           {
                _logger.LogError(ex, "خطا در ارسال پیامک خوش‌آمدگویی.");
                return false;
           }
        }
        public async Task SaveSmsAsync(string phoneNumber, string template, string message, string contact, int smsCount, string status, int? orderId = null, int? factorId = null)
        {
            var sms = new Sms
            {
                PhoneNumber = phoneNumber,
                Template = template,
                Message = message,
                Contact = contact,
                OrderId = null,
                FactorId = null,
                SmsCount = smsCount,
                Status = status
            };

            await _smsRepository.AddAsync(sms);
            await _smsRepository.SaveChangesAsync();
        }


        // متدی برای ارسال پیامک برای مشتری
        public async Task<bool> SendSMSOrderForCustomerAsync(string cellPhone, int orderId)
        {
            //string equalParts;
            //var order = await _orderRepository.GetOrderSummaryByOrderIdAsync(orderId);
            //var s = _orderWidthPartRepository.GetStringAsync(orderId);
            //var message = $"{order.Count} عدد پرده آهنربایی {order.CategoryGroup} به ابعاد {order.SizeSMS} به صورت {order.PartCount} قسمت {order.IsEqualParts} از چپ به راست{order.WidthParts} با کد پیگیری {order.OrderId} برای شما ثبت گردید.";

            //if (order.IsEqualParts)
            //{
            //    equalParts = "مساوی";
            //}
            //else
            //{
            //    equalParts = "نامساوی";
            //}
            //try
            //{
            //    var otpsms = new Api(_smsSettings.ApiKey);
            //    var result = await otpsms.VerifyAsync(1, "MadWinGetOrderForCustomer",
            //        new string[]
            //        { cellPhone },
            //        order.Count.ToString(),
            //        order.CategoryGroup,
            //        order.SizeSMS,
            //        order.PartCount + " قسمت " + equalParts + " از راست به چپ " + s, order.OrderId.ToString());

            //    if (result != null)
            //    {
            //        await SaveSmsAsync(cellPhone, "MadWinGetOrderForCustomer", message, "سفارش دهنده", 4, "200", orderId, null);
            //        _logger.LogInformation("پیامک ثبت سفارش توسط مشتری با موفقیت ارسال شد.");
            //        return true;
            //    }
            //    else
            //    {
            //        _logger.LogWarning("ارسال پیامک ثبت سفارش توسط مشتری با خطا مواجه شد. نتیجه خالی بود.");
            //        return false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "خطا در ارسال پیامک سفارش مشتری‌.");
            //    return false;
            //}
            return true;
        }

        // متدی برای ارسال پیامک برای تولید
        public async Task<bool> SendSMSOrderForProductionAsync(string cellPhone, int orderId)
        {
            ////2 عدد پرده آهنربایی طلقی توری به ابعاد 230 * 120 به صورت 2 قسمت نامساوی از چپ به راست(10 - 10) با کد پیگیری 1252 .
            ////با تشکر وین ماد
            //// لغو 11
            //string equalParts;
            //var order = await _orderRepository.GetOrderSummaryByOrderIdAsync(orderId);
            //var s = _orderWidthPartRepository.GetStringAsync(orderId);
            //var message = $"{order.Count} عدد پرده آهنربایی {order.CategoryGroup} به ابعاد {order.SizeSMS} به صورت {order.PartCount} قسمت {order.IsEqualParts} از چپ به راست{order.WidthParts} با کد پیگیری {order.OrderId} برای شما ثبت گردید.";

            //if (order.IsEqualParts)
            //{
            //    equalParts = "مساوی";
            //}
            //else
            //{
            //    equalParts = "نامساوی";
            //}

            //try
            //{
            //    var otpsms = new Api(_smsSettings.ApiKey);
            //    var result = await otpsms.VerifyAsync(1, "MadWinGetOrderForProduction",
            //        new string[]
            //        { cellPhone },
            //        order.Count.ToString(),
            //        order.CategoryGroup,
            //        order.SizeSMS,
            //        order.PartCount + " قسمت " + equalParts + " از راست به چپ " + s, order.OrderId.ToString());


            //    if (result != null)
            //    {
            //        await SaveSmsAsync(cellPhone, "MadWinGetOrderForProduction", message, "تولید کننده", 4, "200", orderId, null);
            //        _logger.LogInformation("پیامک ثبت سفارش برای تولید کننده با موفقیت ارسال شد.");
            //        return true;
            //    }
            //    else
            //    {
            //        _logger.LogWarning("ارسال پیامک ثبت سفارش برای مدیر با خطا مواجه شد. نتیجه خالی بود.");
            //        return false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "خطا در ارسال پیامک سفارش مشتری‌.");
            //    return false;
            //}
            return true;
        }

        //
        public async Task<bool> SendSMSOrderForManagerAsync(string cellPhone, int orderId)
        {
            ////2 عدد پرده آهنربایی طلقی توری به ابعاد 230 * 120 به صورت 2 قسمت نامساوی از چپ به راست(10 - 10) با کد پیگیری 1252 .
            ////با تشکر وین ماد
            //// لغو 11
            //string equalParts;
            //var order = await _orderRepository.GetOrderSummaryByOrderIdAsync(orderId);
            //var s = _orderWidthPartRepository.GetStringAsync(orderId);
            //var message = $"{order.Count} عدد پرده آهنربایی {order.CategoryGroup} به ابعاد {order.SizeSMS} به صورت {order.PartCount} قسمت {order.IsEqualParts} از چپ به راست{order.WidthParts} با کد پیگیری {order.OrderId} برای شما ثبت گردید.";


            //if (order.IsEqualParts)
            //{
            //    equalParts = "مساوی";
            //}
            //else
            //{
            //    equalParts = "نامساوی";
            //}

            //try
            //{
            //    var otpsms = new Api(_smsSettings.ApiKey);
            //    var result = await otpsms.VerifyAsync(1, "MadWinGetOrderForManager",
            //        new string[]
            //        { cellPhone },
            //        order.Count.ToString(),
            //        order.CategoryGroup,
            //        order.SizeSMS,
            //        order.PartCount + " قسمت " + equalParts + " از راست به چپ " + s, order.OrderId.ToString(), order.FullName);


            //    if (result != null)
            //    {
            //        await SaveSmsAsync(cellPhone, "MadWinGetOrderForManager", message, "مدیریت", 4, "200", orderId, null);
            //        _logger.LogInformation("پیامک ثبت سفارش برای تولید کننده با موفقیت ارسال شد.");
            //        return true;
            //    }
            //    else
            //    {
            //        _logger.LogWarning("ارسال پیامک ثبت سفارش برای تولید کننده با خطا مواجه شد. نتیجه خالی بود.");
            //        return false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "خطا در ارسال پیامک سفارش مشتری‌.");
            //    return false;
            //}
            return false;
        }


        public async Task<bool> SendSMSFactorForCustomerAsync(string cellPhone, int factorId)
        {
            var message = $"با موفقیت ثبت شد." + factorId + "فاکتور شما با کد پیگیری.\r\nبا تشکر از خرید شما مادوین(نادری)\r\n لغو 11";
            try
            {
                var otpsms = new Api(_smsSettings.ApiKey);
                var result = otpsms.VerifyAsync(1, "MadWinGetFactorForCustomer",
                new string[] { cellPhone },//شماره خریدار
                factorId.ToString(), "ماد وین");

                if (result != null)
                {
                    await SaveSmsAsync(cellPhone, "MadWinGetFactorForCustomer", message, "سفارش دهنده", 2, "200",null,factorId);
                    _logger.LogInformation("پیامک ثبت فاکتور توسط مشتری با موفقیت ارسال شد.");
                    return true;
                }
                else
                {
                    _logger.LogWarning("ارسال پیامک ثبت فاکتور توسط مشتری با خطا مواجه شد. نتیجه خالی بود.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ارسال پیامک سفارش مشتری‌.");
                return false;
            }
        }

        public async Task<bool> SendSMSFactorForManagerAsync(int factorId)
        {

            //var message = $"Message = \"فاکتور\" + item.FactorId + \"-\" + listFactorDetails.Count + \"-\" + i + \" \" + \"نوع\" + item.Product.Title + \" \" + \"تعداد :\" + item.Quantity + \" \" + \"با تشکر پناه پلاست لغو 11\",";
            var factorDetails = await _factorService.GetFactorDetailForSendSMSAsync(factorId);

            if (factorDetails == null)
            {
                _logger.LogWarning("فاکتور یافت نشد.");
                return false;
            }

            try
            {
                var i = 1;
                foreach (var item in factorDetails.Details)
                {
                    var otpsms = new Api(_smsSettings.ApiKey);

                    // ساخت پیامک (مثلا می‌تونی هر متنی می‌خوای بسازی)
                    string message = $"محصول: {item.ProductName}, تعداد: {item.Quantity}, سفارش‌دهنده: {factorDetails.FullName}, فاکتور: {factorDetails.FactorId}-{i}";

                    var result = await otpsms.VerifyAsync(1, "MadWinGetFactorForManager",
                                      new string[] { "09180580270" }, // شماره مقصد
                                      message,
                                      $"{factorDetails.FactorId}-{item.Quantity}-{i}", // شناسه پیامک یا کد دلخواه
                                      item.ProductName,
                                      item.Quantity.ToString(),
                                      "ماد وین");

                    if (result != null)
                    {
                        await SaveSmsAsync("09180580270", "MadWinGetFactorForCustomer", message, "سفارش دهنده", 2, "200", null, factorId);
                        _logger.LogInformation($"پیامک محصول {item.ProductName} با موفقیت ارسال شد.");
                    }
                    else
                    {
                        _logger.LogWarning($"ارسال پیامک محصول {item.ProductName} با خطا مواجه شد. نتیجه خالی بود.");
                        return false;
                    }

                    i++;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ارسال پیامک فاکتور");
                return false;
            }



        }
       
        public async Task<bool> SendSMSForgotPasswordForCustomer(string cellPhone, string password)
        {
            var message = $"کلمه عبور جدید {password}اگر شما این درخواست را انجام نداده‌اید، لطفاً فوراً با پشتیبانی تماس بگیرید.با تشکر مادوین(نادری)لغو 11";
            try
            {
                var otpsms = new Api(_smsSettings.ApiKey);
                var result = await otpsms.VerifyAsync(1, "MadWinForgotPassword",
                    new string[]
                    { cellPhone },
                    password);


                if (result != null)
                {
                    await SaveSmsAsync(cellPhone, "MadWinForgotPassword", message, "مشتری", 1, "200", null, null);
                    _logger.LogInformation("پیامک تغییر کلمه عبور با موفقیت انجام شد.");
                    return true;
                }
                else
                {
                    _logger.LogWarning("پیامک تغییر کلمه عبور با موفقیت انجام نشد. نتیجه خالی بود.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ارسال پیامک تغییر کلمه عبور‌.");
                return false;
            }

        }

        public async Task<int> CountSendSms()
        {
            var sendSms = await _smsRepository.GetAllAsync();
            return sendSms.Count();
        }

        public async Task<UserForAdminViewModel> GetAllUsers(int pageId = 1, string filterFirstName = "")
        {
            return null;// await _userRepository.GetAllUsers(pageId, filterFirstName);
        }
    }
}


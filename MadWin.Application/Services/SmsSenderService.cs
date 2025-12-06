using Ghasedak.Core;
using MadWin.Core.DTOs.SendSms;
using MadWin.Core.DTOs.Users;
using MadWin.Core.Entities.SentMessages;
using MadWin.Core.Interfaces;
using MadWin.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop2City.Core.Senders;

namespace MadWin.Application.Services
{
    public class SmsSenderService : ISmsSenderService
    {
        private readonly ISmsRepository _smsRepository;
        private readonly ILogger<SmsSenderService> _logger;
        private readonly SmsSettings _smsSettings;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderWidthPartRepository _orderWidthPartRepository;
        private readonly IFactorService _factorService;

        public SmsSenderService(
            ISmsRepository smsRepository,
            ILogger<SmsSenderService> logger,
            IOptions<SmsSettings> smsSettings,
            IOrderRepository orderRepository,
            IOrderWidthPartRepository orderWidthPartRepository,
            IFactorService factorService)
        {
            _smsRepository = smsRepository;
            _logger = logger;
            _smsSettings = smsSettings.Value;
            _orderRepository = orderRepository;
            _orderWidthPartRepository = orderWidthPartRepository;
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
        public async Task SaveSmsAsync(string phoneNumber, string template, string message, string contact, int smsCount, string status, int? orderId , int? factorId)
        {
            var sms = new Sms
            {
                PhoneNumber = phoneNumber,
                Template = template,
                Message = message,
                Contact = contact,
                OrderId = orderId,
                FactorId = factorId,
                SmsCount = smsCount,
                Status = status
            };

            await _smsRepository.AddAsync(sms);
            await _smsRepository.SaveChangesAsync();
        }



        #region ارسال سفارش برای مشتری
        public async Task<bool> SendSMSOrderForCustomerAsync(string cellPhone, int orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrdersByOrderIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning($"سفارشی با شناسه {orderId} یافت نشد.");
                    return false;
                }

                var widthPart =  _orderWidthPartRepository.GetStringAsync(orderId);
                if (string.IsNullOrWhiteSpace(widthPart))
                {
                    _logger.LogWarning($"قسمت‌های عرض برای سفارش {orderId} یافت نشد.");
                    widthPart = "-";
                }

                string equalParts = order.IsEqualParts ? "مساوی" : "نامساوی";

                string message =
                    $"{order.Count} عدد پرده آهنربایی {order.CategoryGroup} به ابعاد {order.SizeSMS} " +
                    $"به صورت {order.PartCount} قسمت {equalParts} از راست به چپ {widthPart} " +
                    $"با کد پیگیری {order.OrderId} برای شما ثبت گردید.";

                var otpsms = new Api(_smsSettings.ApiKey);

                var result = await otpsms.VerifyAsync(
                    1,
                    "MadWinGetOrderForCustomer",
                    new[] { cellPhone },
                    order.Count.ToString(),
                    order.CategoryGroup,
                    order.SizeSMS,
                    $"{order.PartCount} قسمت {equalParts} از راست به چپ {widthPart}",
                    order.OrderId.ToString()
                );

                if (result != null)
                {
                    await SaveSmsAsync(cellPhone, "MadWinGetOrderForCustomer", message, "سفارش دهنده", 4, "200", orderId, null);
                    _logger.LogInformation("پیامک ثبت سفارش توسط مشتری با موفقیت ارسال شد.");
                    return true;
                }

                _logger.LogWarning("ارسال پیامک ثبت سفارش توسط مشتری با خطا مواجه شد. نتیجه خالی بود.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ارسال پیامک سفارش مشتری.");
                return false;
            }
        }

        #endregion
    
        #region ارسال فاکتور برا مشتری
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
                    await SaveSmsAsync(cellPhone, "MadWinGetFactorForCustomer", message, "سفارش دهنده", 2, "200", null, factorId);
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
        #endregion
        #region ارسال فاکتور برای اقای نادری
        public async Task<bool> SendSMSFactorForManagerAsync(int factorId)
        {
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


                    //تغییر دادم توی مجل کار
                    //1404-08-11
                    //تعداد کل زیر فاکتور
                    // ساخت پیامک (مثلا می‌تونی هر متنی می‌خوای بسازی)
                    string message = $"محصول: {item.ProductName}, تعداد: {item.Count}, سفارش‌دهنده: {factorDetails.FullName}, فاکتور: {factorDetails.FactorId}-{factorDetails.Details.Count}-{i}";


                    var result = await otpsms.VerifyAsync(1, "MadWinGetFactorForManager",
                                      new string[] { "09180580270" }, // شماره مقصد
                                      factorDetails.FullName,
                                      $"{factorDetails.FactorId}-{factorDetails.Details.Count}-{i}", //شماره فاکتور + تعداد زیر فاکتور + ردیف فاکتور
                                      item.ProductName,
                                      item.Count.ToString(),// تعدااد سفارشی مشتری
                                      "ماد وین");

                    if (result != null)
                    {
                        await SaveSmsAsync("09180580270", "MadWinGetFactorForManager", message, "مدیریت", 2, "200", null, factorId);
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

        #endregion
        #region ارسال فاکتور برای خانم حیدری
        public async Task<bool> SendSMSFactorForProductionAsync(int factorId)
        {
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
                    string message = $"محصول: {item.ProductName}, تعداد: {item.Count}, فاکتور: {factorDetails.FactorId}-{factorDetails.Details.Count}-{i}";

                    var result = await otpsms.VerifyAsync(1, "MadWinGetFactorForProduction",
                                      new string[] { "09182185223" }, // شماره مقصد
                                      $"{factorDetails.FactorId}-{factorDetails.Details.Count}-{i}",
                                      item.ProductName,
                                      item.Count.ToString(),
                                      "ماد وین");

                    if (result != null)
                    {
                        await SaveSmsAsync("09182185223", "MadWinGetFactorForProduction", message, "تولید کننده", 2, "200", null, factorId);
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

        #endregion
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

        public Task<bool> SendSMSOrderForProductionAsync(string cellPhone, int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendSMSOrderForManagerAsync(string cellPhone, int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<SendSmsForAdminViewModel> GetAllSendSms(int pageId)
        {
            return await _smsRepository.GetAllSendSms(pageId);
        }
    }
}


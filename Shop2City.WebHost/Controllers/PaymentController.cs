using MadWin.Application.Services;
using MadWin.Core.Entities.Advices;
using MadWin.Core.Entities.Factors;
using MadWin.Core.Entities.Orders;
using MadWin.Core.Entities.SentMessages;
using MadWin.Core.Entities.Transactions;
using MadWin.Core.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Shop2City.Core.Services.Transactions;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Shop2City.WebHost.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        private readonly ITransactionService _transactionService;
        private readonly IFactorService _factorService;
        private readonly IOrderService _orderService;
        private readonly ISmsSenderService _smsSenderService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IUserService userService,
            ITransactionService transactionService,
            IFactorService factorService,
            IOrderService orderService,
            ISmsSenderService smsSenderService,
            ILogger<PaymentController> logger)
        {
            _userService = userService;
            _transactionService = transactionService;
            _factorService = factorService;
            _orderService = orderService;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(300) };
            _smsSenderService = smsSenderService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
           // try
            //{
                // user validation (optional) — فقط برای اعتبارسنجی کاربر
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                {
                    // اگر می‌خوای اجباری نباشه، می‌تونی این را برداری. الان برای امنیت بررسی می‌کنیم.
                    return Json(new { success = false, message = "کاربر نامعتبر" });
                }

                // Callback شامل source و deliveryId فرستاده می‌شود تا در Verify قابل دسترسی باشد
                string callbackUrl = $"https://madwin.ir/Payment/Verify?source={request.Source}&deliveryId={request.deliveryId}";
                string url = "https://sepehr.shaparak.ir:8081/V1/PeymentApi/GetToken";

                var paymentRequest = new
                {
                    TerminalID = "98808771",
                    Amount = request.sumPrice,
                    InvoiceID = request.InvoiceId,
                    callbackURL = callbackUrl,
                    payload = ""
                };


                #region درج توضیحات
                if (request.Source == "order")
                {
                    await _orderService.AddDescriptionForOrder(request.InvoiceId, request.Description);
                }
                else
                {
                    await _factorService.AddDescriptionForFactor(request.InvoiceId, request.Description);
                }
                #endregion

                string jsonData = JsonSerializer.Serialize(paymentRequest);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.PostAsync(url, content);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در ارسال درخواست به درگاه پرداخت");
                    return Json(new { success = false, message = "خطا در ارتباط با درگاه پرداخت" });
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("درگاه پرداخت پاسخ غیرموفق داد. StatusCode: {StatusCode}", response.StatusCode);
                    return Json(new { success = false, message = "خطا در دریافت پاسخ از درگاه پرداخت" });
                }

                var responseData = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(responseData);
                var token = jsonObject["Accesstoken"]?.ToString();

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("توکن از درگاه پرداخت دریافت نشد. Response: {Response}", responseData);
                    return Json(new { success = false, message = "توکن از درگاه پرداخت دریافت نشد." });
                }

                // آدرس redirect صحیح (نام پارامترها را بر اساس مستند درگاه خودت چک کن)
                string redirectUrl = $"https://sepehr.shaparak.ir:8080/Pay?token={token}&terminalID=98808771&getMethod=0";

                // نکته مهم: هیچ‌گونه Update روی سفارش/فاکتور و یا ارسال پیامک در این متد انجام نمی‌دهیم.
                // این کارها فقط پس از تایید Advice در متد Verify انجام خواهند شد.


                return Json(new { success = true, redirectUrl = redirectUrl });
           // }
            //catch (Exception ex)
            //{
                //_logger.LogError(ex, "خطا در ProcessPayment");
                //return Json(new { success = false, message = "خطای داخلی سرور" });
           // }
        }

        [HttpGet]
        public async Task<IActionResult> Verify()
        {

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                // اگر می‌خوای اجباری نباشه، می‌تونی این را برداری. الان برای امنیت بررسی می‌کنیم.
                return Json(new { success = false, message = "کاربر نامعتبر" });
            }

            //try
            //{
            //    if (!Request.HasFormContentType)
            //    {
            //        _logger.LogWarning("Verify: درخواست بدون فرم دریافت شد.");
            //        return BadRequest("داده‌های فرم نادرست دریافت شده است.");
            //    }

            //    var source = Request.Query["source"].ToString();
            //    var deliveryIdQuery = Request.Query["deliveryId"].ToString();

  

            //    ////parse invoiceId safely
            //    if (!int.TryParse(Request.Form["InvoiceId"], out int invoiceId))
            //    {
            //        _logger.LogWarning("InvoiceId در پاسخ درگاه معتبر نیست.");
            //        return BadRequest("InvoiceId معتبر نیست.");
            //    }

            //    var amountStr = Request.Form["Amount"].ToString();
            //    var digitalReceipt = Request.Form["DigitalReceipt"].ToString();
            //    var datePaidStr = Request.Form["DatePaid"].ToString();
            //    var terminalId = Request.Form["TerminalId"].ToString();
            //    var cardNumber = Request.Form["CardNumber"].ToString();
            //    var payload = Request.Form["Payload"].ToString();
            //    var rrn = Request.Form["Rrn"].ToString();
            //    var respMsg = Request.Form["RespMsg"].ToString();
            //    var traceNumber = Request.Form["TraceNumber"].ToString();
            //    var respCode = Request.Form["RespCode"].ToString();
            //    var issuerBank = Request.Form["IssuerBank"].ToString();

            //    // ارسال درخواست Advice برای تایید نهایی تراکنش
            //    var url = "https://sepehr.shaparak.ir:8081/V1/PeymentApi/Advice";
            //    var advice = new
            //    {
            //        digitalreceipt = digitalReceipt,
            //        Tid = terminalId
            //    };

            //    var jsonData = JsonSerializer.Serialize(advice);
            //    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            //    HttpResponseMessage adviceResponse;
            //    try
            //    {
            //        adviceResponse = await _httpClient.PostAsync(url, content);
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "خطا در ارسال درخواست Advice به درگاه");
            //        // در صورت خطای شبکه، بهتر است کاربر رو به صفحه خطا هدایت کنیم
            //        return RedirectToAction("PaymentFailed", "Payment");
            //    }

            //    if (!adviceResponse.IsSuccessStatusCode)
            //    {
            //        _logger.LogWarning("Advice پاسخ غیرموفق داد. StatusCode: {StatusCode}", adviceResponse.StatusCode);
            //        return RedirectToAction("PaymentFailed", "Payment");
            //    }

            //    var adviceResponseData = await adviceResponse.Content.ReadAsStringAsync();
            //    var adviceJson = JObject.Parse(adviceResponseData);

            //    var adviceModel = new AdviceModel
            //    {
            //        Status = adviceJson["Status"]?.ToString(),
            //        ReturnId = adviceJson["ReturnId"]?.ToString(),
            //        Message = adviceJson["Message"]?.ToString(),
            //    };

            //    await _transactionService.AddAdvice(adviceModel);

            //    // بررسی وضعیت Advice — فقط در صورت Status == "200" (یا هر مقدار موفق مستندات درگاه) ادامه می‌دهیم
            //    if (adviceModel.Status != "Ok")
            //    {
            //        _logger.LogWarning("Advice برگشتی وضعیت موفق ندارد. Status: {Status}, Message: {Message}", adviceModel.Status, adviceModel.Message);
            //        return RedirectToAction("PaymentFailed", "Payment");
            //    }

            //    // ثبت تراکنش در جدول تراکنش‌ها
            //    var transactionModel = new TransactionModel
            //    {
            //        DigitalReceipt = digitalReceipt,
            //        Amount = amountStr,
            //        CardNumber = cardNumber,
            //        DatePaid = datePaidStr,
            //        InvoiceId = invoiceId,
            //        IssuerBank = issuerBank,
            //        Payload = payload,
            //        RespCode = respCode,
            //        RespMsg = respMsg,
            //        Rrn = rrn,
            //        TerminalId = terminalId,
            //        TraceNumber = traceNumber
            //    };
            //    await _transactionService.AddTransaction(transactionModel);

                // --- اکنون تراکنش موفق است: انجام عملیات نهایی روی Order/Factor و ارسال پیامک ---
                // ابتدا پارس deliveryId (که از ProcessPayment در querystring فرستاده شده)
                int deliveryId = 1;
            //if (!string.IsNullOrEmpty(deliveryIdQuery))
            //    int.TryParse(deliveryIdQuery, out deliveryId);

            int invoiceId = 1594;
            var source = "order";

                if (source == "order")
                {
                    // به‌روز‌رسانی سفارش و نهایی‌سازی
                    try
                    {
                        // آپدیت قیمت روش ارسال
                        await _orderService.UpdatePriceAndDeliveryAsync(deliveryId, invoiceId);

                        var order = await _orderService.GetOrderByOrderIdAsync(invoiceId);
                        if (order == null)
                        {
                            _logger.LogWarning("Order یافت نشد برای Invoice: {InvoiceId}", invoiceId);
                            return RedirectToAction("PaymentFailed", "Payment");
                        }

                        await _orderService.UpdateIsFinalyOrderAsync(userId);

                        // شماره مشتری را تلاش می‌کنیم از سرویس کاربر بگیریم (اگر order شامل UserId باشد)
                        string cellPhone = null;
                        try
                        {
                            if (order.UserId > 0)
                                cellPhone = await _userService.GetCellPhoneByUserIdAsync(order.UserId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "دریافت شماره موبایل کاربر با خطا مواجه شد. OrderId: {OrderId}", order.Id);
                        }

                        //ارسال پیامک به مشتری
                        var smsCustomerSent = false;
                        if (!string.IsNullOrEmpty(cellPhone))
                        {
                            smsCustomerSent = await _smsSenderService.SendSMSOrderForCustomerAsync(cellPhone, order.Id);
                            _logger.LogInformation(
                                $"ارسال پیامک به مشتری برای ثبت سفارش جدید (order). OrderId: {order.Id}, Sent: {smsCustomerSent}");

                        }
                        else
                        {
                            _logger.LogWarning("شماره مشتری برای ارسال پیامک سفاش موجود نیست. OrderId: {OrderId}", order.Id);
                        }
                        ////ارسال پیامک به مدیریت
                        var smsManagerSent = await _smsSenderService.SendSMSFactorForManagerAsync(order.Id);
                        _logger.LogInformation("ارسال پیامک به مدیر پس از پرداخت (factor). FactorId: {FactorId}, Sent: {Sent}", order.Id, smsManagerSent);

                        ////ارسال پیامک به تولیدی
                        var smsProductionSent = await _smsSenderService.SendSMSFactorForProductionAsync(order.Id);
                        _logger.LogInformation("ارسال پیامک به تولیدی پس از پرداخت (factor). FactorId: {FactorId}, Sent: {Sent}", order.Id, smsManagerSent);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطا در نهایی‌سازی فاکتور پس از پرداخت. InvoiceId: {InvoiceId}", invoiceId);
                    }

                    return RedirectToAction("Index", "Orders", new { area = "UserPanel", orderId = invoiceId });
                }
                else if (source == "factor")
                {
                    try
                    {
                        await _factorService.UpdatePriceAndDeliveryAsync(deliveryId, invoiceId);

                        var factor = await _factorService.GetFactorByFactorIdAsync(invoiceId);
                        if (factor == null)
                        {
                            _logger.LogWarning("Factor یافت نشد برای Invoice: {InvoiceId}", invoiceId);
                            return RedirectToAction("PaymentFailed", "Payment");
                        }

                        await _factorService.UpdateIsFinalyFactorAsync(factor.Id);

                        // دریافت شماره مشتری
                        string cellPhone = null;
                        try
                        {
                            if (factor.UserId > 0)
                                cellPhone = await _userService.GetCellPhoneByUserIdAsync(factor.UserId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "دریافت شماره موبایل کاربر برای فاکتور با خطا مواجه شد. FactorId: {FactorId}", factor.Id);
                        }
                        //ارسال پیامک به مشتری
                        var smsCustomerSent = false;
                        if (!string.IsNullOrEmpty(cellPhone))
                        {
                            smsCustomerSent = await _smsSenderService.SendSMSFactorForCustomerAsync(cellPhone, factor.Id);
                            _logger.LogInformation("ارسال پیامک به مشتری پس از پرداخت (factor). FactorId: {FactorId}, Sent: {Sent}", factor.Id, smsCustomerSent);
                        }
                        else
                        {
                            _logger.LogWarning("شماره مشتری برای ارسال پیامک فاکتور موجود نیست. FactorId: {FactorId}", factor.Id);
                        }
                        //ارسال پیامک به مدیریت
                        var smsManagerSent = await _smsSenderService.SendSMSFactorForManagerAsync(factor.Id);
                        _logger.LogInformation("ارسال پیامک به مدیر پس از پرداخت (factor). FactorId: {FactorId}, Sent: {Sent}", factor.Id, smsManagerSent);

                        //ارسال پیامک به تولیدی
                        var smsProductionSent = await _smsSenderService.SendSMSFactorForProductionAsync(factor.Id);
                        _logger.LogInformation("ارسال پیامک به تولیدی پس از پرداخت (factor). FactorId: {FactorId}, Sent: {Sent}", factor.Id, smsManagerSent);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "خطا در نهایی‌سازی فاکتور پس از پرداخت. InvoiceId: {InvoiceId}", invoiceId);
                    }

                    return RedirectToAction("Index", "Factors", new { area = "UserPanel", factorId = invoiceId });
                }

                _logger.LogWarning("پارامتر source نامعتبر در Verify: {Source}", source);
                return BadRequest("پارامتر source نامعتبر است.");
           // }
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "خطا در Verify");
            //    return RedirectToAction("PaymentFailed", "Payment");
            //}
        }

        public IActionResult PaymentFailed()
        {
            return View();
        }
        public class PaymentRequest
        {
            public int sumPrice { get; set; }
            public int InvoiceId { get; set; }
            public string Source { get; set; }
            public int deliveryId { get; set; }

            public string Description { get; set; }
        }
    }
}


using Azure;
using MadWin.Core.DTOs.SendSms;
using MadWin.Core.DTOs.Users;
using MadWin.Core.Entities.SentMessages;
using MadWin.Core.Entities.Users;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MadWin.Infrastructure.Repositories
{
    public class SmsRepository : GenericRepository<Sms>, ISmsRepository
    {
        private readonly MadWinDBContext _context;

        public SmsRepository(MadWinDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sms>> GetRecentSmsAsync(int count)
        {
            return await _context.SentMessages
                .OrderByDescending(s => s.CreateDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sms>> GetFailedSmsAsync()
        {
            return await _context.SentMessages
                .Where(s => s.Status == "Failed")
                .ToListAsync();
        }

        public Task<bool> SendWelcomeSmsAsync(string cellPhone, string fullName, int smsCount)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendSMSOrderForCustomerAsync(string cellPhone, int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendSMSOrderForProductionAsync(string cellPhone, int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendSMSOrderForManagerAsync(string cellPhone, int orderId)
        {
            throw new NotImplementedException();
        }

        public Task SaveSmsAsync(string phoneNumber, string template, string message, string contact, int smsCount, string status, int? orderId = null, int? factorId = null)
        {
            throw new NotImplementedException();
        }
        public async Task<SendSmsForAdminViewModel> GetAllSendSms(int pageId)
        {
            IQueryable<Sms> result = GetQuery()
                .IgnoreQueryFilters()
                .Where(u => !u.IsDelete);

            int take = 10;
            int skip = (pageId - 1) * take;

            // شمارش کل رکوردها با async
            int totalCount = await result.CountAsync();

            // محاسبه تعداد صفحات
            int countPage = (int)Math.Ceiling(totalCount / (double)take);

            // اگر pageId از آخرین صفحه بیشتر بود، خودش را برابر آخرین صفحه کن
            if (countPage > 0 && pageId > countPage)
            {
                pageId = countPage;
                skip = (pageId - 1) * take;
            }

            var list = new SendSmsForAdminViewModel
            {
                CurrentPage = pageId,
                CountPage = countPage,
                SendSms = await result
                    .OrderByDescending(u => u.Id)
                    .Skip(skip)
                    .Take(take)
                    .Select(u => new SendSmsForAdminItemViewModel
                    {
                        Id = u.Id,
                        CreateDate = u.CreateDate,
                        Template = u.Template,
                        Contact = u.Contact,
                        FactorId = u.FactorId,
                        OrderId=u.OrderId,
                        SmsCount = u.SmsCount,
                        Status = u.Status,
                        Message = u.Message,
                        PhoneNumber = u.PhoneNumber
                    })
                    .ToListAsync()
            };

            return list;
        }


    }

}

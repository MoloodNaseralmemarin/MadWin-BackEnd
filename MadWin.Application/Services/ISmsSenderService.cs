using MadWin.Core.DTOs.SendSms;
using MadWin.Core.DTOs.Users;
using Shop2City.Core.Senders;

namespace MadWin.Application.Services
{
    public interface ISmsSenderService
    {
        Task<bool> SendWelcomeSmsAsync(string cellPhone, string fullName, int smsCount);
        Task<bool> SendSMSOrderForCustomerAsync(string cellPhone, int orderId);
        Task<bool> SendSMSOrderForProductionAsync(string cellPhone, int orderId);
        Task<bool> SendSMSOrderForManagerAsync(string cellPhone, int orderId);


        Task<bool> SendSMSFactorForCustomerAsync(string cellPhone, int factorId);

        Task<bool> SendSMSFactorForManagerAsync(int factorId);

        Task<bool> SendSMSFactorForProductionAsync(int factorId);

        Task<bool> SendSMSForgotPasswordForCustomer(string cellPhone,string password);

        Task SaveSmsAsync(string phoneNumber, string template, string message, string contact, int smsCount, string status, int? orderId = null, int? factorId = null);

        Task<int> CountSendSms();

        Task<SendSmsForAdminViewModel> GetAllSendSms(int pageId);
    }
}


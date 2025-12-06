using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Core.DTOs.SendSms
{
    #region UserForAdminViewModel

    public class SendSmsForAdminItemViewModel
    {
        public int Id { get; set; }
        public DateTime CreateDate {  get; set; }
        public string PhoneNumber { get; set; }
        public int SmsCount { get; set; } = 2;
        public string Message { get; set; }
        public string Contact { get; set; }
        public int? OrderId { get; set; }
        public int? FactorId { get; set; }
        public string Status { get; set; }
        public string Template { get; set; }
    }
    public class SendSmsForAdminViewModel
    {
        public List<SendSmsForAdminItemViewModel> SendSms { get; set; }

        public int CurrentPage { get; set; }
        public int CountPage { get; set; }
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Application.DTOs.CommissionRates
{
    public class CommissionRateDto
    {
        public int Id { get; set; }
        public int PartCount { get; set; }              // تعداد تکه (2 یا 3)
        public bool IsEqualParts { get; set; }          // آیا تکه‌ها مساوی‌اند؟
        public int CommissionPercent { get; set; }   // درصد کارمزد

        public DateTime LastUpdateDate { get; set; }
    }
}

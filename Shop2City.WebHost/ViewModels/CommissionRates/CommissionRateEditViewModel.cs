using System.ComponentModel.DataAnnotations;

namespace Shop2City.WebHost.ViewModels.CommissionRates
{
    public class CommissionRateEditViewModel
    {
        public int Id { get; set; }
        public int PartCount { get; set; }              // تعداد تکه (2 یا 3)
        public bool IsEqualParts { get; set; }          // آیا تکه‌ها مساوی‌اند؟

        [Display(Name ="درصد کارمزد")]
        public int CommissionPercent { get; set; }   // درصد کارمزد

        public string Description { get; set; }

        public bool IsDelete { get; set; }

        public DateTime LastUpdateDate { get; set; }
    }
}

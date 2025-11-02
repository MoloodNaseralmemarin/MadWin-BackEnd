using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Core.DTOs.DisCounts
{
    public class DiscountForAdminItemViewModel
    {
        public int Id { get; set; }

        public DateTime CreateDate { get; set; }
        [Display(Name = "عنوان")]
        public string Item { get; set; } //O سسفارش با انداره دلخواه -D// سفارش آماده

        [Display(Name = "کد تخفیف")]
        public string DiscountCode { get; set; }

        [Display(Name = "درصد کد تخفیف")]
        public int Percentage { get; set; }

        [Display(Name = "تعداد اولیه")]
        public int InitialCount { get; set; }

        [Display(Name = "تعداد")]
        public int UseableCount { get; set; }

        [Display(Name = "تاریخ شروع")]
        public DateTime StartDate { get; set; }

        [Display(Name = "تاریخ پایان")]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "فعال/غیرفعال")]
        public bool IsInactive
        {
            get
            {
                var now = DateTime.Now;
                return now < StartDate || now > ExpiryDate;
            }
        }


        public string Description {  get; set; }
    }

    public class DiscountForAdminViewModel
    {
        public List<DiscountForAdminItemViewModel> Discounts { get; set; }

        public int CurrentPage { get; set; }
        public int CountPage { get; set; }
    }

    public class DiscountResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal NewTotal { get; set; }
    }

    public class AddDiscountForAdminViewModel
    {
        [Display(Name = "عنوان")]
        public string Item { get; set; } 

        [Display(Name = "کد تخفیف")]
        public string DiscountCode { get; set; }

        [Display(Name = "درصد کد تخفیف")]
        public int Percentage { get; set; }


        [Display(Name = "تعداد اولیه")]
        public int InitialCount { get; set; }

        [Display(Name = "تعداد")]
        public int UseableCount { get; set; }

        [Display(Name = "تاریخ شروع")]
        public DateTime StartDate { get; set; }

        [Display(Name = "تاریخ پایان")]
        public DateTime ExpiryDate { get; set; }

    }
}

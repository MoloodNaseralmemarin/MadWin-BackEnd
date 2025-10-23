using MadWin.Core.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Shop2City.WebHost.ViewModels.Orders
{
    public class OrderViewModel
    {

        public int UserId { get; set; }
        [Display(Name = "دسته‌بندی")]
        [Required(ErrorMessage = "لطفاً یک دسته‌بندی انتخاب کنید.")]
        public int? CategoryId { get; set; }

        [Display(Name = "زیر دسته‌بندی")]
        [Required(ErrorMessage = "لطفاً یک زیر دسته‌بندی انتخاب کنید.")]
        public int? SubCategoryId { get; set; }

        public SelectList Categories { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
        public SelectList SubCategories { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        [Display(Name = "ارتفاع")]
        [Required(ErrorMessage = "ارتفاع را وارد کنید.")]
        //[Range(1, int.MaxValue, ErrorMessage = "مقدار ارتفاع باید بیشتر از صفر باشد.")]
        public int? Height { get; set; }

        [Display(Name = "عرض")]
        [Required(ErrorMessage = "عرض را وارد کنید.")]
        //[Range(1, int.MaxValue, ErrorMessage = "مقدار عرض باید بیشتر از صفر باشد.")]
        public int? Width { get; set; }

        [Display(Name = "تعداد")]
        [Required(ErrorMessage = "تعداد را وارد کنید.")]
        [Range(1, int.MaxValue, ErrorMessage = "مقدار تعداد باید بیشتر از صفر باشد.")]
        public int? Count { get; set; }

        // برای تقسیم‌بندی
        public int PartCount { get; set; }
        public bool IsEqualParts { get; set; }

        public string DivisionType { get; set; }

        // عرض بخش‌های جداگانه
        public List<int> WidthParts { get; set; } = new List<int>();
    }

}
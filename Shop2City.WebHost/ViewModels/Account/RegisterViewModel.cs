using System.ComponentModel.DataAnnotations;

namespace Shop2City.WebHost.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفاً نام را وارد کنید")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفاً نام خانوادگی را وارد کنید")]
        public string LastName { get; set; }

        [Display(Name = "شماره همراه")]
        [Required(ErrorMessage = "شماره همراه الزامی است")]
        [MaxLength(11)]
        public string CellPhone { get; set; }

        [MaxLength(11)]
        [Display(Name = "شماره ثابت")]
        public string? TelPhone { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "کلمه عبور را وارد کنید")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "کلمه‌های عبور مطابقت ندارند")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "آدرس پستی")]
        public string? Address { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}

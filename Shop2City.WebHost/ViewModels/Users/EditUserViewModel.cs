using MadWin.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Shop2City.WebHost.ViewModels.Users
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "نام", Prompt = "نام")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(100, ErrorMessage = ErrorMessage.MaxLength)]
        public string FirstName { get; set; }


        [Display(Name = "نام خانوادگی", Prompt = "نام خانوادگی")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(100, ErrorMessage = ErrorMessage.MaxLength)]
        public string LastName { get; set; }


        [Display(Name = "شماره همراه", Prompt = "شماره همراه")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(11, ErrorMessage = ErrorMessage.MaxLength)]
        public string CellPhone { get; set; }


        [Display(Name = "شماره ثابت", Prompt = "شماره همراه")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(11, ErrorMessage = ErrorMessage.MaxLength)]
        public string TellPhone { get; set; }


        [Display(Name = "آدرس پستی")]
        [MaxLength(500, ErrorMessage = ErrorMessage.MaxLength)]
        public string Address { get; set; }


        [Display(Name = "کلمه عبور", Prompt = "کلمه عبور")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(100, ErrorMessage = ErrorMessage.MaxLength)]
        public string Password { get; set; }

        [Display(Name = "تکرار کلمه عبور", Prompt = "تکرار کلمه عبور")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(100, ErrorMessage = ErrorMessage.MaxLength)]
        [Compare("Password", ErrorMessage = ErrorMessage.Compare)]
        public string ConfirmPassword { get; set; }
    }
}

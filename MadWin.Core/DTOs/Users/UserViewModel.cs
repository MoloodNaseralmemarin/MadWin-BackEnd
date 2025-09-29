using MadWin.Core.Common;
using MadWin.Core.Entities.Users;
using System.ComponentModel.DataAnnotations;


namespace MadWin.Core.DTOs.Users
{
    #region UserForAdminViewModel

    public class UserForAdminItemViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string CellPhone { get; set; }

        public DateTime CreateDate { get; set; }

        public int OrderCount { get; set; }
        public int FactorCount { get; set; }
    }
    public class UserForAdminViewModel
    {
        public List<UserForAdminItemViewModel> Users { get; set; }

        public int currentPage { get; set; }
        public int countPage { get; set; }
    }

    #endregion
    #region CreateUserViewModel
    public class CreateUserViewModel
    {
        [Display(Name = "نام", Prompt = "نام")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(100, ErrorMessage = ErrorMessage.MaxLength)]
        public string firstName { get; set; }

        [Display(Name = "نام خانوادگی", Prompt = "نام خانوادگی")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(100, ErrorMessage = ErrorMessage.MaxLength)]
        public string lastName { get; set; }

        [Display(Name = "شماره همراه", Prompt = "شماره همراه")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(11, ErrorMessage = ErrorMessage.MaxLength)]
        public string cellPhone { get; set; }

        [Display(Name = "شماره ثابت", Prompt = "شماره ثابت")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(11, ErrorMessage = ErrorMessage.MaxLength)]
        public string tellPhone { get; set; }


        [Display(Name = "نام کاربری", Prompt = "نام کاربری")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(100, ErrorMessage = ErrorMessage.MaxLength)]
        public string? userName { get; set; }

        [Display(Name = "کلمه عبور", Prompt = "کلمه عبور")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(100, ErrorMessage = ErrorMessage.MaxLength)]
        public string password { get; set; }

 

        [Display(Name = "آدرس پستی",Prompt ="آدرس پستی")]
        [MaxLength(500, ErrorMessage = ErrorMessage.MaxLength)]
        public string address { get; set; }

    }
    #endregion

    #region EditUserViewModel
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
        public string TelPhone { get; set; }


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
    #endregion

}

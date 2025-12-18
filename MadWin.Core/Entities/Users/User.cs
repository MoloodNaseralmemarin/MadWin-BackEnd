using System.ComponentModel.DataAnnotations;
using MadWin.Core.Entities.Orders;
using MadWin.Core.Entities.Common;
using MadWin.Core.Entities.Factors;
using MadWin.Core.Common;

namespace MadWin.Core.Entities.Users
{
    public class User : BaseEntity
    {
        #region Ctor
        public User()
        {
            UserRoles = new();
            Factors = new();
            Orders = new();
        }
        #endregion
        #region Field

        [Display(Name = "نام")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MinLength(3, ErrorMessage = ErrorMessage.MinLength)]
        [MaxLength(100, ErrorMessage = ErrorMessage.MaxLength)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MinLength(3, ErrorMessage = ErrorMessage.MinLength)]
        [MaxLength(100, ErrorMessage = ErrorMessage.MaxLength)]
        public string LastName { get; set; }

        [Display(Name = "شماره همراه")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(11, ErrorMessage = ErrorMessage.MaxLength)]
        public string CellPhone { get; set; }

        [Display(Name = "شماره ثابت")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(11, ErrorMessage = ErrorMessage.MaxLength)]
        public string TelPhone { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        public string UserName { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        public string Password { get; set; }

        [Display(Name = "آدرس پستی")]
        [Required(ErrorMessage = ErrorMessage.Required)]
        [MaxLength(500, ErrorMessage = ErrorMessage.MaxLength)]
        public string Address { get; set; }

        #endregion

        #region Relationship

        public virtual List<UserRole> UserRoles { get; set; }
        public virtual List<Factor> Factors { get; set; }

        public virtual List<Order> Orders { get; set; }

        #endregion
    }
}

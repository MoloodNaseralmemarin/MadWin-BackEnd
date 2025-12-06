
using MadWin.Core.Common;
using System.ComponentModel.DataAnnotations;


namespace Shop2City.WebHost.ViewModels.CurtainComponents
{
    public class CurtainComponentEditViewModel
    {
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = ErrorMessage.Required)]

        public string Name { get; set; }
        [Display(Name = "قیمت (ریال)")]
        [Required(ErrorMessage =ErrorMessage.Required)]
        public decimal Cost { get; set; }
    }
}

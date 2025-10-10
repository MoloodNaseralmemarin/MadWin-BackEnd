using System.ComponentModel.DataAnnotations;

namespace Shop2City.WebHost.ViewModels.CurtainComponents
{
    public class CurtainComponentEditViewModel
    {
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        public string Name { get; set; }
        [Display(Name = "قیمت")]
        public decimal Cost { get; set; }
    }
}

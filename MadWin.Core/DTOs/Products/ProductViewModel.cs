using MadWin.Core.Common;
using MadWin.Core.Entities.Products;
using MadWin.Core.Entities.Properties;
using System.ComponentModel.DataAnnotations;

namespace MadWin.Core.DTOs.Products
{
    #region UserForAdminViewModel

    public class ProductForAdminItemViewModel
    {
        public int ProductId { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public string Title { get; set; }
        public decimal Price { get; set; }
        public string ImageName { get; set; }

        public int FactorCount {  get; set; }

        [Display(Name = "موجود / ناموجود")]
        public bool IsStatus { get; set; }
    }
    public class ProductForAdminViewModel
    {
        public List<ProductForAdminItemViewModel> Products { get; set; }

        public int CurrentPage { get; set; }
        public int CountPage { get; set; }
    }

    #endregion


    public class ShowProductListItemViewModel
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string ShortDescription { get; set; }

        public decimal Price { get; set; }

        public string FileName { get; set; }

        public bool IsStatus { get; set; }

    }


}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Application.DTOs.Products
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string ImageName { get; set; }
        public decimal PricePerUnit { get; set; }

        public int count = 1;
    }

    public class EditProductDto
    {
        public int Id { get; set; }

        [Display(Name ="محصول")]
        public string Title { get; set; }

        [Display(Name = "قیمت")]
        public int Price { get; set; }

        public string ShortDescription { get; set; }
    }
}

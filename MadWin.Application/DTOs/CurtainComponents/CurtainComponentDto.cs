using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Application.DTOs.CommissionRates
{
    public class CurtainComponentDto
    {
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        public string Name { get; set; }
        [Display(Name = "قیمت")]
        public decimal Cost { get; set; }

        public string Description { get; set; }

        public bool IsDelete { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

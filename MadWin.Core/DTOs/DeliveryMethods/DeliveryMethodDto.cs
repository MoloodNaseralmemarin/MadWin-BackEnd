using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Core.DTOs.DeliveryMethods
{
    public class DeliveryMethodForAdmin
    {
        public int Id { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public string Name { get; set; }

        public decimal Cost { get; set; }
    }

    public class EditDeliveryMethodForAdmin
    {
        public int Id { get; set; }

        [Display(Name="عنوان")]
        public string Name { get; set; }
        [Display(Name = "قیمت(ریال)")]
        public decimal Cost { get; set; }
    }
}

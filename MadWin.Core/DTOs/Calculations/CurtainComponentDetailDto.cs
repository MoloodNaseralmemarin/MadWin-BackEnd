using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Core.DTOs.Calculations
{
    public class CurtainComponentDetailDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CurtainComponentId { get; set; }

        public string ComponentName { get; set; }
        public decimal UnitCost { get; set; }
        public int Count { get; set; }
        public decimal FinalCost { get; set; }

        // از جدول Order
        public int Width { get; set; }
        public int Height { get; set; }

        public decimal BasePrice { get; set; }
        public decimal TotalAmount { get; set; }

        public string CategoryName { get; set; }
    }
}

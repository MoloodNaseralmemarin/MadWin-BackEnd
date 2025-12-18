using System;
using System.Collections.Generic;
using System.Text;

namespace MadWin.Core.DTOs.Orders
{
    public class DailyOrderSummaryDto
    {
        public DateTime? Date { get; set; }
        public string FullName { get; set; }
        public string CellPhone { get; set; }

        public int FinalOrderCount { get; set; }
        public decimal FinalTotalPrice { get; set; }

        public int OpenOrderCount { get; set; }
        public decimal OpenTotalPrice { get; set; }

        public bool IsFinaly { get; set; }

        // این اضافه میشه
        public List<OrderDetailDto> Orders { get; set; } = new();
    }

    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public DateTime CreateDate { get; set; }
        public string FullName { get; set; }
        public string CellPhone { get; set; }
        public string Address { get; set; }
        public string DeliveryMethodName { get; set; }
        public decimal DeliveryMethodCost { get; set; }
        public int DisPercent { get; set; }
        public decimal DisTotal { get; set; }
        public string CategoryGroup { get; set; }
        public string Size { get; set; }
        public int Count { get; set; }
        public bool IsCurtainAdhesive { get; set; }

        public bool IsEqualParts { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsFinaly { get; set; }
        public string Description { get; set; }
        public List<OrderWidthPartDto> WidthParts { get; set; } = new();
    }

}

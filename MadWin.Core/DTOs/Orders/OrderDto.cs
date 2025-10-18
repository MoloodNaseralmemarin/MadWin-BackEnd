using MadWin.Core.Common;
using MadWin.Core.DTOs.Users;
using MadWin.Core.Lookups.DeliveryMethods;
using System.ComponentModel.DataAnnotations;


namespace MadWin.Core.DTOs.Orders
{
    public class OrderSummaryForAdminItemDto
    {
        public int OrderId { get; set; }
        public DateTime CreateDate { get; set; }
        public string FullName { get; set; }


        public string Address { get; set; }


        /// <summary>درصد کد تخفیف</summary>
        public int DisPercent { get; set; }

        /// <summary>قیمت تخفیف محاسبه شده</summary>
        public decimal DisTotal { get; set; }

        public string CellPhone { get;set; }
        public string CategoryGroup { get; set; }
        public string Size { get; set; }
        public string SizeSMS { get; set; }
        public int Count { get; set; }
        public int PartCount { get; set; }
        public bool IsEqualParts { get; set; }
        public decimal PriceWithFee { get; set; }
        public bool IsFinaly { get; set; }

        public decimal BasePrice { get; set; }

        public decimal TotalPrice { get; set; }

        public List<OrderWidthPartDto> WidthParts { get; set; } = new();
    }

    public class OrderWidthPartDto
    {
        public int WidthValue { get; set; }
    }

    public class OrderSummaryForAdminDto
    {
        public List<OrderSummaryForAdminItemDto> OrderSummary { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }

    public class OrderSummaryViewModel
    {
        public OrderSummaryForAdminDto OrderSummaryForAdmin { get; set; }
        public IEnumerable<DeliveryMethodInfoLookup> DeliveryMethods { get; set; }

    }


    public class OrderForAdminViewModel
    {
        public List<OrderSummaryForAdminItemDto> OrderSummary { get; set; } = new();
        public OrderFilterParameters filter { get; set; }

        public int CurrentPage { get; set; }
        public int CountPage { get; set; }
    }



}

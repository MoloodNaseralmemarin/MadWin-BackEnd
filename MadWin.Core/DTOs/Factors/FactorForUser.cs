namespace MadWin.Core.DTOs.Factors
{

    public class FactorSummaryForUserItemDto
    {
        public int FactorId { get; set; }

        public DateTime CreateDate { get; set; }

        public string CellPhone { get; set; }

        // لیست جزئیات (هر محصول)


        public string Address { get; set; }

        /// <summary>درصد کد تخفیف</summary>
        public int DisPercent { get; set; }

        /// <summary>قیمت تخفیف محاسبه شده</summary>
        public decimal DisTotal { get; set; }

        // لیست جزئیات (هر محصول)
        public List<FactorDetailForUserDto> FactorDetails { get; set; } = new();


        // مبلغ تخفیف
        public decimal Discount { get; set; }

        public string DeliveryMethodName { get; set; }

        // هزینه ارسال
        public decimal DeliveryMethodCost { get; set; }

        public bool IsFinaly { get; set; }

        public decimal TotalAmount { get; set; }

        public int FactorDetailItemCount { get; set; }

        public string Description { get; set; }

    }

    public class FactorDetailForUserDto
    {
        public int FactorDetailId { get; set; }
        public string ProductTitle { get; set; } = "";
        public int Count { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }

    }

    public class FactorForUserViewModel
    {
        public List<FactorSummaryForUserItemDto> FactorSummaryForUser { get; set; } = new();
        public FactorFilterParameter Filter { get; set; }

        public int CurrentPage { get; set; }
        public int CountPage { get; set; }
    }
}
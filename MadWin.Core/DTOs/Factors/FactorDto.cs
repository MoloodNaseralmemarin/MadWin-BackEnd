using MadWin.Core.Lookups.DeliveryMethods;

namespace MadWin.Core.DTOs.Factors

{
public class FactorSummaryForAdminItemDto
{
    public int FactorId { get; set; }

    public DateTime CreatedAt { get; set; }
    public string FullName { get; set; }

    
    public string CellPhone { get; set; }

    // لیست جزئیات (هر محصول)


   public string Address { get; set; }
        
    /// <summary>درصد کد تخفیف</summary>
    public int DisPercent { get; set; }

    /// <summary>قیمت تخفیف محاسبه شده</summary>
    public decimal DisTotal { get; set; }

        // لیست جزئیات (هر محصول)
    public List<FactorDetailDto> FactorDetails { get; set; } = new();

    // جمع کل قبل از تخفیف
    public decimal SubTotal  {get;set;}

    // مبلغ تخفیف
    public decimal Discount { get; set; }

   /// <summary>
   /// نحوه ارسال
   /// </summary>
   public string DeliveryMethodName { get; set; }
        // هزینه ارسال
   public decimal DeliveryMethodCost { get; set; }

    // مبلغ قابل پرداخت = جمع کل - تخفیف + ارسال
    public decimal FinalTotal => SubTotal - Discount + DeliveryMethodCost;

    public bool IsFinaly { get; set; }
    public int FactorDetailItemCount { get; set; }

   public decimal TotalAmount { get; set; }
   
   public string Description { get; set; }


  }

public class FactorDetailDto
{
    public int Id { get; set; }
    public string ProductTitle { get; set; } = "";
    public int Count { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsNew { get; set; }

}

public class FactorForAdminViewModel
{
    public List<FactorSummaryForAdminItemDto> FactorSummary { get; set; } = new();
    public FactorFilterParameter filter { get; set; }

    public int CurrentPage { get; set; }
    public int CountPage { get; set; }
}

public class FactorSummaryForAdminDto
{
    public List<FactorSummaryForAdminItemDto> FactorSummary { get; set; } = new();
    public decimal TotalCost { get; set; }
}
public class FactorSummaryViewModel
{
    public FactorSummaryForAdminDto FactorSummaryForAdmin { get; set; }
    public IEnumerable<DeliveryMethodInfoLookup> DeliveryMethods { get; set; }

}
    public class FactorSummaryForSendSMS
    {
        public string FullName { get; set; }
        public int FactorId { get; set; }
        public List<FactorDetailItemForSendSMS> Details { get; set; } = new List<FactorDetailItemForSendSMS>();
    }

    public class FactorDetailItemForSendSMS
    {
        public string ProductName { get; set; }
        public int Count { get; set; }
    }

}
using MadWin.Core.DTOs.FilterParameters;
using MadWin.Core.Lookups.DeliveryMethods;

namespace MadWin.Core.DTOs.Fators

{
public class FactorSummaryForAdminItemDto
{
    public int FactorId { get; set; }

    public DateTime CreateDate { get; set; }
    public string FullName { get; set; }

    // لیست جزئیات (هر محصول)
    public List<FactorDetailDto> FactorDetails { get; set; } = new();

    // جمع کل قبل از تخفیف
    public decimal SubTotal  {get;set;}

    // مبلغ تخفیف
    public decimal Discount { get; set; }

    // هزینه ارسال
    public decimal DeliveryPrice { get; set; }

    // مبلغ قابل پرداخت = جمع کل - تخفیف + ارسال
    public decimal FinalTotal => SubTotal - Discount + DeliveryPrice;

    public bool IsFinaly { get; set; }
    public int FactorDetailItemCount { get; set; }


}

public class FactorDetailDto
{
    public int Id { get; set; }
    public string ProductTitle { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice { get; set; }

}

public class FactorForAdminViewModel
{
    public List<FactorSummaryForAdminItemDto> FactorSummary { get; set; } = new();
    public FilterParameter filter { get; set; }

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
}
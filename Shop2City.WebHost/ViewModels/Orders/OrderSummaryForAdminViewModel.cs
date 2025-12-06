using MadWin.Core.DTOs.Factors;
using MadWin.Core.DTOs.Orders;

namespace Shop2City.WebHost.ViewModels.Orders
{
    public class OrderSummaryForAdminViewModel
    {
        public FactorFilterParameter Filter { get; set; }
        public PagedResult<OrderSummaryForAdminViewModel> OrderSummary { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}

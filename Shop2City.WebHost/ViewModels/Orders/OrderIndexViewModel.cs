
using MadWin.Core.DTOs.Factors;
using MadWin.Core.DTOs.Orders;

namespace Shop2City.WebHost.ViewModels.Orders
{
    public class OrderIndexViewModel
    {
        public PagedResult<OrderSummaryForAdminDto> Orders { get; set; }
        public FactorFilterParameter Filter { get; set; }
    }
}

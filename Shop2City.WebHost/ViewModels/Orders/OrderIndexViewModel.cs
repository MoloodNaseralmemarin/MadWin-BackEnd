using MadWin.Core.DTOs.Orders;

namespace Shop2City.WebHost.ViewModels.Orders
{
    public class OrderIndexViewModel
    {
        public PagedResult<OrderSummaryDto> Orders { get; set; }
        public OrderFilterParameters Filter { get; set; }
    }
}

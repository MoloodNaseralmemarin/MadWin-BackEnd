using MadWin.Core.DTOs.Orders;
using MadWin.Core.Lookups.DeliveryMethods;
using Shop2City.WebHost.ViewModels.DeliveryMethods;

namespace Shop2City.WebHost.ViewModels.Orders
{
    public class OrderSummaryPageViewModel
    {
        public PagedResult<OrderSummaryDto> Orders { get; set; }
        public IEnumerable<DeliveryMethodInfoLookup> DeliveryMethods { get; set; }
        public OrderSummaryDto OrderSummary { get; set; }
    }


}

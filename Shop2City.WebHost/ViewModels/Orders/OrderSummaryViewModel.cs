using MadWin.Core.DTOs.Orders;
using MadWin.Core.Lookups.DeliveryMethods;
using Shop2City.WebHost.ViewModels.DeliveryMethods;

namespace Shop2City.WebHost.ViewModels.Orders
{
    public class OrderSummaryPageViewModel
    {
        public IEnumerable<DeliveryMethodInfoLookup> DeliveryMethods { get; set; } = new List<DeliveryMethodInfoLookup>();

        public PagedResult<OrderSummaryDto> OrderSummary { get; set; } = new PagedResult<OrderSummaryDto>
        {
            Items = new List<OrderSummaryDto>(),
            TotalCount = 0
        };
    }


}

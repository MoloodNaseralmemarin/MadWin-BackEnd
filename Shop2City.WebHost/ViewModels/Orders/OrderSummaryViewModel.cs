using MadWin.Core.DTOs.Orders;
using MadWin.Core.Lookups.DeliveryMethods;

namespace Shop2City.WebHost.ViewModels.Orders
{
    public class OrderSummaryViewModel
    {
        public IEnumerable<DeliveryMethodInfoLookup> DeliveryMethods { get; set; }

        public OrderSummaryForAdminViewModel OrderSummaryForAdmin{ get;set;}

    }


}

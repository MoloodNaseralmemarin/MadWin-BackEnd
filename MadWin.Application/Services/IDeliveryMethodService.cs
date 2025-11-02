using MadWin.Application.DTOs.DeliveryMethods;
using MadWin.Core.DTOs.DisCounts;
using MadWin.Core.Entities.DeliveryMethods;
using MadWin.Core.Interfaces;
using MadWin.Core.Lookups.DeliveryMethods;

namespace MadWin.Application.Services
{
    public interface IDeliveryMethodService
    {
        Task<IEnumerable<DeliveryMethodInfoLookup>> GetDeliveryMethodInfoAsync();
        Task<decimal> GetDeliveryMethodCostByIdAsync(int deliveryId);

        //Task<DeliveryMethod> GetAllDeliveryMeth
            
        //    odsAsync();
    }
}

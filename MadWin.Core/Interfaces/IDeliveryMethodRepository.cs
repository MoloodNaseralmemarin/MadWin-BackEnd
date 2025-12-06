
using MadWin.Core.DTOs.DeliveryMethods;
using MadWin.Core.Entities.DeliveryMethods;
using MadWin.Core.Lookups.DeliveryMethods;

namespace MadWin.Core.Interfaces
{
    public interface IDeliveryMethodRepository : IGenericRepository<DeliveryMethod>
    {
        Task<IEnumerable<DeliveryMethodInfoLookup>> GetDeliveryMethodInfoAsync();

        Task<decimal> GetDeliveryMethodCostByIdAsync(int deliveryId);

        Task<DeliveryMethodInfoLookup> GetDeliveryMethodInfoAsync(int deliveryId);
        Task<IEnumerable<DeliveryMethodForAdmin>> GetDeliveryMethodForAdminAsync();

        Task<EditDeliveryMethodForAdmin> GetDeliveryMethodByIdAsync(int id);

    }
}

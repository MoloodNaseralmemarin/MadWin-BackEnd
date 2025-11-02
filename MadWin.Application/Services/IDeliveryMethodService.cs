using MadWin.Core.DTOs.DeliveryMethods;
using MadWin.Core.Lookups.DeliveryMethods;
using System.Threading.Tasks;

namespace MadWin.Application.Services
{
    public interface IDeliveryMethodService
    {
        Task<IEnumerable<DeliveryMethodInfoLookup>> GetDeliveryMethodInfoAsync();
        Task<decimal> GetDeliveryMethodCostByIdAsync(int deliveryId);

        Task<IEnumerable<DeliveryMethodForAdmin>> GetDeliveryMethodForAdminAsync();
        Task<EditDeliveryMethodForAdmin> GetDeliveryMethodByIdAsync(int id);
        Task<bool> EditDeliveryMethodAsync(EditDeliveryMethodForAdmin editDeliveryMethod);
    }
}

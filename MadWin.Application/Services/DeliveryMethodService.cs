
using MadWin.Application.DTOs.Products;
using MadWin.Application.Services;
using MadWin.Core.DTOs.DeliveryMethods;
using MadWin.Core.Interfaces;
using MadWin.Core.Lookups.DeliveryMethods;


namespace MadWin.Application.Repositories
{
    public class DeliveryMethodService : IDeliveryMethodService
    {
        private readonly IDeliveryMethodRepository _deliveryMethodRepository;

        public DeliveryMethodService(IDeliveryMethodRepository deliveryMethodRepository)
        {
            _deliveryMethodRepository = deliveryMethodRepository;
        }

        public async Task<decimal> GetDeliveryMethodCostByIdAsync(int deliveryId)
        {
            return await _deliveryMethodRepository.GetDeliveryMethodCostByIdAsync(deliveryId);
        }

        public async Task<IEnumerable<DeliveryMethodForAdmin>> GetDeliveryMethodForAdminAsync()
        {
            return await _deliveryMethodRepository.GetDeliveryMethodForAdminAsync();
        }

        public async Task<IEnumerable<DeliveryMethodInfoLookup>> GetDeliveryMethodInfoAsync()
        {
            return await _deliveryMethodRepository.GetDeliveryMethodInfoAsync();
        }

        public async Task<EditDeliveryMethodForAdmin> GetDeliveryMethodByIdAsync(int id)
        {
            return await _deliveryMethodRepository.GetDeliveryMethodByIdAsync(id);
        }

        public async Task<bool> EditDeliveryMethodAsync(EditDeliveryMethodForAdmin editDeliveryMethod)

        {
            var existing = await _deliveryMethodRepository.GetByIdAsync(editDeliveryMethod.Id);
            if (existing == null) return false;

            existing.Name = editDeliveryMethod.Name;
            existing.Cost = editDeliveryMethod.Cost;
            existing.LastUpdateDate = DateTime.Now;

            await _deliveryMethodRepository.SaveChangesAsync();
            return true;
        }
    }
}

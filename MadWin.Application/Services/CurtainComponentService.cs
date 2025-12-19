using MadWin.Application.DTOs.CommissionRates;
using MadWin.Core.Entities.CurtainComponents;
using MadWin.Core.Interfaces;

namespace MadWin.Application.Services
{
    public class CurtainComponentService : ICurtainComponentService
    {
        public readonly ICurtainComponentRepository _curtainComponentRepository;

        public CurtainComponentService(ICurtainComponentRepository curtainComponentRepository)
        {
           _curtainComponentRepository = curtainComponentRepository;     
        }

        public async Task<bool> EditCurtainComponentAsync(CurtainComponent curtainComponent)
        {
            var existing = await _curtainComponentRepository.GetByIdAsync(curtainComponent.Id);
            if (existing == null) return false;

            existing.Name = curtainComponent.Name;
            existing.Cost = curtainComponent.Cost;
            existing.UpdatedAt = DateTime.UtcNow;

            await _curtainComponentRepository.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<CurtainComponentDto>> GetAllCurtainComponentAsync()
        {
            var curtainComponent = await _curtainComponentRepository.GetAllAsync();
            return curtainComponent.Select(c => new CurtainComponentDto
            {
                Id = c.Id,
                Cost = c.Cost,
                Description = c.Description,
                IsDelete = c.IsDelete,
                UpdatedAt = c.UpdatedAt,
                Name= c.Name
            });
        }

        public async Task<CurtainComponent> GetByIdAsync(int id)
        {
            return await _curtainComponentRepository.GetByIdAsync(id);
        }
    }
}

using MadWin.Application.DTOs.CommissionRates;
using MadWin.Core.Entities.CurtainComponents;

namespace MadWin.Application.Services
{
    public interface ICurtainComponentService
    {
        Task<IEnumerable<CurtainComponentDto>> GetAllCurtainComponentAsync();

        Task<bool> EditCurtainComponentAsync(CurtainComponent curtainComponent);

        Task<CurtainComponent> GetByIdAsync(int id);
    }
}

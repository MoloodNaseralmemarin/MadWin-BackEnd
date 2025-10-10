
using MadWin.Core.Entities.CurtainComponents;

namespace MadWin.Application.Services
{
    public interface ICurtainComponentDetailService
    {
        Task<CurtainComponentDetail> CreateCurtainComponentDetailInitialAsync(int orderId, int curtainComponentId, decimal unitCost, int count);
    }
}

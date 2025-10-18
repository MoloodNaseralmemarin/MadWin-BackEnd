using MadWin.Core.DTOs.Calculations;
using MadWin.Core.Entities.CurtainComponents;

namespace MadWin.Core.Interfaces
{
    public interface ICurtainComponentDetailRepository:IGenericRepository<CurtainComponentDetail>
    {

        Task<IEnumerable<CurtainComponentDetailDto>> GetCurtainComponentDetailByOrderIdAsync(int orderId);

    }
}

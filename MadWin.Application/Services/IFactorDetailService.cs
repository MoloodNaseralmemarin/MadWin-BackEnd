using MadWin.Core.DTOs.Fators;
using MadWin.Core.DTOs.FilterParameters;
using MadWin.Core.DTOs.Orders;
using MadWin.Core.Entities.CurtainComponents;
using MadWin.Core.Interfaces;


namespace MadWin.Application.Services
{
    public interface IFactorDetailService
    {
        Task<FactorSummaryForAdminDto> GetOpenFactorAsync(int userId, int factorId);

        Task SoftDeleteAsync(IEnumerable<int> factorDetailIds);

        Task<FactorForAdminViewModel> GetAllFactorAsync(FilterParameter filter, int pageId = 1);

        Task<FactorForAdminViewModel> GetAllFactorByUserIdAsync(int userId,FilterParameter filter, int pageId = 1);

        Task<IEnumerable<FactorDetailDto>> GetByFactorIdAsync(int factorId);
        Task MarkItemsAsSeenAsync(int factorId);




    }
}

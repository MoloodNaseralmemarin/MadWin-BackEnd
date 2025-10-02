using MadWin.Core.DTOs.FilterParameters;
using MadWin.Core.Entities.CurtainComponents;
using MadWin.Core.Interfaces;


namespace MadWin.Application.Services
{
    public interface IFactorDetailService
    {
        Task<FactorForAdminViewModel> GetFactorSummaryByFactorIdAsync(int factorId, int userId);

        Task SoftDeleteAsync(IEnumerable<int> factorDetailIds);

        Task<FactorForAdminViewModel> GetAllFactorAsync(FilterParameter filter, int pageId = 1);

        Task<IEnumerable<FactorDetailDto>> GetByFactorIdAsync(int factorId);




    }
}

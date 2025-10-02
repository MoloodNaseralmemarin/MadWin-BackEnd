using MadWin.Core.DTOs.FilterParameters;
using MadWin.Core.Entities.Factors;

namespace MadWin.Core.Interfaces
{
    public interface IFactorDetailRepository:IGenericRepository<FactorDetail>
    {
        Task<FactorDetail> AddFactorDetailAsync(int factorId, int count, int productId);
        Task<decimal> FactorSum(int factorId);
        Task<bool> IsExistFactorDetailAsync(int factorId);
        Task<FactorDetail> GetFactorByfactorIdAsync(int factorId);

        Task<List<FactorDetail>> GetAllFactorDetailByFactorIdAsync(int factorId);

        Task<FactorDetail?> GetFactorDetailByProductIdAsync(int factorId, int productId);

        decimal GetSubtotalByFactorId(int factorId);
        Task<FactorForAdminViewModel> GetAllFactorAsync(FilterParameter filter, int pageId = 1);
        Task<FactorForAdminViewModel> GetFactorSummaryByFactorIdAsync(int factorId, int userId);

        Task<IEnumerable<FactorDetailDto>> GetByFactorIdAsync(int factorId);

    }
}

using MadWin.Core.DTOs.Factors;



namespace MadWin.Application.Services
{
    public interface IFactorDetailService
    {
        Task<FactorSummaryForAdminDto> GetOpenFactorAsync(int userId, int? factorId);

        Task SoftDeleteAsync(IEnumerable<int> factorDetailIds);

        Task<FactorForAdminViewModel> GetAllFactorAsync(FactorFilterParameter filter, int pageId = 1);

        Task<FactorForUserViewModel> GetAllFactorByUserIdAsync(int userId,FactorFilterParameter filter, int pageId = 1);

        Task<IEnumerable<FactorDetailDto>> GetByFactorIdAsync(int factorId);
        Task MarkItemsAsSeenAsync(int? factorId);




    }
}

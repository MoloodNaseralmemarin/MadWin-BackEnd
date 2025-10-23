using MadWin.Core.DTOs.Factors;
using MadWin.Core.DTOs.Fators;
using MadWin.Core.Entities.Factors;

namespace MadWin.Application.Services
{
    public interface IFactorService
    {
        Task<int> AddFactorAsync(int userId, int productId, int count);

        Task<Factor> GetFactorByFactorIdAsync(int factorId);

        Task UpdatePriceAndDeliveryAsync(int deliveryId, int factorId);
        Task UpdateIsFinalyFactorAsync(int factorId);

        Task<int> CreateOrGetOpenFactorAsync(int userId);
        Task AddOrUpdateFactorDetailAsync(int factorId, int productId, int count);
        Task UpdateFactorSumAsync(int factorId);
        decimal GetSubtotal(int factorId);

        Task<int> CountFactors();

        

        Task<FactorSummaryForSendSMS> GetFactorDetailForSendSMSAsync(int factorId);


        Task<FactorSummaryForAdminItemDto> GetFactorSummaryByUserIdAsync(int userId);

        Task<int> GetLastFactorId(int userId);


    }
}



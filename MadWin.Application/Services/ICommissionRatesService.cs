using MadWin.Application.DTOs.CommissionRates;
using MadWin.Core.Entities.CommissionRates;
using MadWin.Core.Entities.CurtainComponents;

namespace MadWin.Application.Services
{
    public interface ICommissionRatesService
    {
        Task<IEnumerable<CommissionRateDto>> GetAllCommissionRateAsync();

        Task<bool> EditCommissionRateAsync(CommissionRate commissionRate);

        Task<CommissionRate> GetByIdAsync(int id);
    }
}

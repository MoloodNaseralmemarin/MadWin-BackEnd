using MadWin.Application.DTOs.CommissionRates;
using MadWin.Core.Entities.CommissionRates;
using MadWin.Core.Entities.CurtainComponents;
using MadWin.Core.Interfaces;

namespace MadWin.Application.Services
{
    public class CommissionRatesService: ICommissionRatesService
    {
        public readonly ICommissionRateRepository _commissionRateRepository;

        public CommissionRatesService(ICommissionRateRepository commissionRateRepository)
        {
            _commissionRateRepository = commissionRateRepository;
        }
        public async Task<bool> EditCommissionRateAsync(CommissionRate commissionRate)
        {
            var existing = await _commissionRateRepository.GetByIdAsync(commissionRate.Id);
            if (existing == null) return false;

            existing.CommissionPercent = commissionRate.CommissionPercent;
            existing.LastUpdateDate = DateTime.UtcNow;

            await _commissionRateRepository.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<CommissionRateDto>> GetAllCommissionRateAsync()
        {
            var commissionRate = await _commissionRateRepository.GetAllAsync();
            return commissionRate.Select(cr => new CommissionRateDto
            {
               Id=cr.Id,
               LastUpdateDate= cr.LastUpdateDate,
               PartCount= cr.PartCount,
               IsEqualParts=cr.IsEqualParts,
               CommissionPercent= cr.CommissionPercent
            });
        }

        public async Task<CommissionRate> GetByIdAsync(int id)
        {
            return await _commissionRateRepository.GetByIdAsync(id);
        }
    }
}
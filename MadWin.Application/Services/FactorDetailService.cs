
using MadWin.Core.DTOs.Factors;
using MadWin.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MadWin.Application.Services
{
    public class FactorDetailService : IFactorDetailService
    {
        private readonly IFactorDetailRepository _factorDetailRepository;
        private readonly IFactorRepository _factorRepository;
        public FactorDetailService(IFactorDetailRepository factorDetailRepository, IFactorRepository factorRepository)
        {
            _factorDetailRepository = factorDetailRepository;
            _factorRepository = factorRepository;
        }
        public async Task<FactorSummaryForAdminDto> GetOpenFactorAsync(int userId, int? factorId)
        {
            return await _factorDetailRepository.GetOpenFactorAsync(userId,factorId);
        }
        public async Task SoftDeleteAsync(IEnumerable<int> factorDetailIds)
        {
            if (factorDetailIds == null || !factorDetailIds.Any())
                return;

            var allFactorDetails = await _factorDetailRepository.GetAllAsync();
            var factorDetailsToDelete = allFactorDetails.Where(fd => factorDetailIds.Contains(fd.Id) && !fd.IsDelete).ToList();

            if (!factorDetailsToDelete.Any())
                return;

            // گروه‌بندی ریز فاکتورهای حذف شده بر اساس فاکتور مربوطه
            var factorIds = factorDetailsToDelete.Select(fd => fd.FactorId).Distinct();

            foreach (var detail in factorDetailsToDelete)
            {
                detail.IsDelete = true;
                detail.UpdatedAt = DateTime.Now;
                detail.Description = "توسط کاربر حذف شده است.";
                _factorDetailRepository.Update(detail);
            }

            await _factorDetailRepository.SaveChangesAsync();

            // حالا قیمت فاکتورها رو بروزرسانی کن
            var allFactors = await _factorRepository.GetAllAsync();

            foreach (var factorId in factorIds)
            {
                // جمع قیمت ریز فاکتورهای حذف نشده برای هر فاکتور
                var sumPrice = allFactorDetails
                    .Where(fd => fd.FactorId == factorId && !fd.IsDelete)
                    .Sum(fd => fd.Price * fd.Count);

                var factor = allFactors.FirstOrDefault(f => f.Id == factorId);
                if (factor != null)
                {
                    factor.SubTotal = sumPrice;
                    factor.UpdatedAt = DateTime.Now;
                    factor.Description = "تغییر قیمت رخ داد چون ایتمی توسط کاربر حذف شده است";
                    _factorRepository.Update(factor);
                }
            }

            await _factorRepository.SaveChangesAsync();
        }

        public async Task<FactorForAdminViewModel> GetAllFactorAsync(FactorFilterParameter filter, int pageId = 1)
        {
            return  await _factorDetailRepository.GetAllFactorAsync(filter, pageId);
        }
        public async Task<FactorForUserViewModel> GetAllFactorByUserIdAsync(int userId, FactorFilterParameter filter, int pageId = 1)
        {
            return await _factorDetailRepository.GetAllFactorByUserIdAsync(userId,filter, pageId);
        }
        public async Task<IEnumerable<FactorDetailDto>> GetByFactorIdAsync(int factorId)
        {
            return await _factorDetailRepository.GetByFactorIdAsync(factorId);
        }

        public async Task MarkItemsAsSeenAsync(int? factorId)
        {
            var newItems = await _factorDetailRepository.GetQuery()
                .Where(d => d.FactorId == factorId && d.IsNew)
                .ToListAsync();

            foreach (var item in newItems)
                item.IsNew = false;

            await _factorDetailRepository.SaveChangesAsync();
        }
    }
}

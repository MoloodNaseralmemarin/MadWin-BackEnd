using MadWin.Core.DTOs.Factors;
using MadWin.Core.Entities.Factors;
using MadWin.Core.Entities.Orders;
using MadWin.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace MadWin.Application.Services
{
    public class FactorService : IFactorService
    {
        private readonly IFactorRepository _factorRepository;
        private readonly IFactorDetailRepository _factorDetailRepository;
        public ILogger<IFactorService> _logger;


        public FactorService(IFactorRepository factorRepository,IFactorDetailRepository factorDetailRepository, ILogger<IFactorService> logger)
        {
            _factorRepository = factorRepository;
            _factorDetailRepository = factorDetailRepository;
            _logger = logger;
        }

        public async Task<int> AddFactorAsync(int userId, int productId, int count)
        {
            // پیدا کردن فاکتور باز کاربر (پرداخت نشده)
            var factor = await _factorRepository.GetOpenFactorByUserIdAsync(userId);

            if (factor == null)
            {
                // اگر فاکتور باز نداشت → یک فاکتور جدید بساز
                factor = await _factorRepository.AddFactorAsync(userId);
            }

            // بررسی کنیم آیا این محصول توی فاکتور باز هست یا نه
            var factorDetail = await _factorDetailRepository.GetFactorDetailByProductIdAsync(factor.Id, productId);

            if (factorDetail == null)
            {
                // اگر محصول وجود نداشت → رکورد جدید اضافه کن
                await _factorDetailRepository.AddFactorDetailAsync(factor.Id, count, productId);
            }
            else
            {
                // اگر محصول وجود داشت → فقط تعدادش رو زیاد کن
                factorDetail.Count += count;
                _factorDetailRepository.Update(factorDetail);
                await _factorDetailRepository.SaveChangesAsync();
            }

            // آپدیت مجموع فاکتور
            await _factorRepository.UpdateFactorSum(factor.Id);

            return factor.Id;
        }

        public async Task<Factor> GetFactorByFactorIdAsync(int factorId)
        {
            return await _factorRepository.GetFactorByFactorIdAsync(factorId);
        }

        public async Task UpdateIsFinalyFactorAsync(int factorId)
        {
            var factor = await _factorRepository.GetByIdAsync(factorId); // یا FindAsync
            if (factor == null) return;

            factor.IsFinaly = true;
            factor.TotalAmount = factor.SubTotal + factor.DeliveryMethodAmount - factor.DisTotal;

            await _factorRepository.SaveChangesAsync();
        }


        public async Task UpdatePriceAndDeliveryAsync(int deliveryId, int factorId)
        {
            try
            {
                await _factorRepository.UpdatePriceAndDeliveryAsync(deliveryId, factorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating factor {FactorId} in OrderService", factorId);
                throw;
            }
        }


        public async Task<int> CreateOrGetOpenFactorAsync(int userId)
        {
            var factor = await _factorRepository.GetOpenFactorByUserIdAsync(userId);
            if (factor == null)
            {
                factor = await _factorRepository.AddFactorAsync(userId);
            }
            return factor.Id;
        }

        public async Task AddOrUpdateFactorDetailAsync(int factorId, int productId, int count)
        {
            var factorDetail = await _factorDetailRepository.GetFactorDetailByProductIdAsync(factorId, productId);

            if (factorDetail == null)
            {
                await _factorDetailRepository.AddFactorDetailAsync(factorId, count, productId);
            }
            else
            {

                factorDetail.Count += count;
                factorDetail.IsNew = true;
                _factorDetailRepository.Update(factorDetail);
                await _factorDetailRepository.SaveChangesAsync();
            }
        }

        public async Task UpdateFactorSumAsync(int factorId)
        {
            await _factorRepository.UpdateFactorSum(factorId);
        }

        public decimal GetSubtotal(int factorId)
        {
            return _factorDetailRepository.GetSubtotalByFactorId(factorId);
        }

        public async Task<int> CountFactors()
        {
            return await _factorRepository.CountFactors();
        }

        public async Task<decimal> GetTotalFactorsPriceAsync()
        {
            return await _factorRepository.GetTotalFactorsPriceAsync();
        }

        public async Task<decimal> GetTodayTotalFactorsPriceAsync()
        {
            return await _factorRepository.GetTodayTotalFactorsPriceAsync();
        }
        public async Task<FactorSummaryForSendSMS> GetFactorDetailForSendSMSAsync(int factorId)
        {
            return await _factorRepository.GetFactorDteailForSendSMSAsync(factorId);
        }



 public async Task<FactorSummaryForAdminItemDto?> GetFactorSummaryByUserIdAsync(int userId)
{
    var factor = await _factorRepository.GetQuery()
        .Include(f => f.FactorDetails)
        .ThenInclude(fd => fd.Product)
        .Include(f => f.User)
        .Include(f => f.DeliveryMethod)
        .FirstOrDefaultAsync(f => f.IsDelete && f.User.Id==userId);

    if (factor == null)
        return null;

    // محاسبه جمع کل
    var subTotal = factor.FactorDetails
        .Where(fd => !fd.IsDelete)
        .Sum(fd => fd.Price);

    // ساخت DTO خروجی
    var dto = new FactorSummaryForAdminItemDto
    {
        FactorId = factor.Id,
        CreateDate = factor.CreateDate,
        FullName = factor.User?.FirstName + factor.User?.LastName ?? "نامشخص",
        CellPhone = factor.User?.CellPhone ?? "-",
        Address = factor.User ?.Address ?? "-",

        DisPercent = factor.DisPercent,
        DisTotal = factor.DisTotal,
       

      //  DeliveryPrice = factor.DeliveryMethod?.Price ?? 0,
        SubTotal = subTotal,

        FactorDetails = factor.FactorDetails
            .Where(fd => !fd.IsDelete)
            .Select(fd => new FactorDetailDto
            {
                Id = fd.Id,
                ProductTitle = fd.Product?.Title ?? "محصول نامشخص",
                Count = fd.Count,
                Price = fd.Price,
                TotalPrice = fd.Price
            }).ToList(),

        FactorDetailItemCount = factor.FactorDetails.Count(fd => !fd.IsDelete),
        IsFinaly = factor.IsFinaly
    };

    return dto;
}

        public async Task<int> GetLastFactorId(int userId)
        {
            return await _factorRepository.GetLastFactorId(userId);  
        }

        public async Task AddDescriptionForFactor(int factorId, string description)
        {
            var factor = await _factorRepository.GetByIdAsync(factorId);
            factor.Description = description;
            await _factorRepository.SaveChangesAsync();
        }

        public async Task<int> GetTodaySettledFactorsCount()
        {
            var today = DateTime.Today;

            return await _factorRepository.GetQuery()
                .Where(o => o.IsFinaly &&
                            o.CreateDate.Date == today &&
                            !o.IsDelete)
                .CountAsync();
        }


        public async Task<int> GetTodayUnSettledFactorsCount()
        {
            var today = DateTime.Today;

            return await _factorRepository.GetQuery()
                .Where(o => !o.IsFinaly &&
                            o.CreateDate.Date == today &&
                            !o.IsDelete)
                .CountAsync();
        }
    }
}
using MadWin.Core.DTOs.Factors;
using MadWin.Core.Entities.Factors;
using MadWin.Core.Interfaces;
using MadWin.Core.Lookups.Factors;
using MadWin.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MadWin.Infrastructure.Repositories
{
    public class FactorRepository : GenericRepository<Factor>, IFactorRepository
    {
        private readonly MadWinDBContext _context;  
        private readonly IFactorDetailRepository _factorDetailRepository;
        private readonly IDeliveryMethodRepository _deliveryMethodRepository;
        public FactorRepository(MadWinDBContext context, IFactorDetailRepository factorDetailRepository, IDeliveryMethodRepository deliveryMethodRepository) : base(context)
        {
            _context = context;
            _factorDetailRepository = factorDetailRepository;
            _deliveryMethodRepository = deliveryMethodRepository;
        }
        public async Task<bool> IsExistOpenFactorAsync(int userId, bool isFinaly)
        {
            return await _context.Set<Factor>().AnyAsync(u => u.UserId == userId && u.IsFinaly==isFinaly);
        }

        public async Task<Factor> AddFactorAsync(int userId)
        {
            Factor factor=new Factor();
            factor.UserId = userId;
            factor.IsFinaly = false;
            factor.SubTotal = 0;
            factor.DeliveryMethodId = 1;
            factor.DeliveryMethodAmount = 0;
            await AddAsync(factor);
            await _context.SaveChangesAsync();
            return factor;
        }
        public async Task UpdateFactorSum(int factorId)
        {
            var factor = await GetByIdAsync(factorId);
            var factorSum =await _factorDetailRepository.FactorSum(factorId);
            factor.SubTotal = factorSum;
            Update(factor);    
            await SaveChangesAsync();
        }

        public async Task<Factor> GetFactorAsync(int userId)
        {
            var factor = await _context.Factors
               .FirstOrDefaultAsync(o => o.UserId == userId && !o.IsFinaly);
            return factor;
        }

        public async Task<Factor> GetFactorByUserIdAsync(int userId)
        {
            var factor =await _context.Set<Factor>()
                            .FirstOrDefaultAsync(o => o.UserId == userId && !o.IsFinaly);
            return factor;

        }

        public async Task<FactorInfoLookup> GetFactorInfoByFactorIdAsync(int factorId)
        {
   
            return await GetQuery()
                      .Where(f => f.Id == factorId)
                      .Where(f => f.FactorDetails.Any(fd => !fd.IsDelete))
                      .Select(f => new FactorInfoLookup
                      {
                          FactorId = f.Id,
                          Price = f.SubTotal
                      })
                      .FirstOrDefaultAsync();
        }

        public async Task<Factor> GetFactorByFactorIdAsync(int factorId)
        {
            return await _context.Set<Factor>().AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == factorId);
        }

        public async Task UpdatePriceAndDeliveryAsync(int deliveryId, int factorId)
        {
            var deliveryMethod = await _deliveryMethodRepository.GetDeliveryMethodInfoAsync(deliveryId);
            var factor = await GetByIdAsync(factorId);
            factor.DeliveryMethodId = deliveryMethod.DeliveryId;
            factor.DeliveryMethodAmount = deliveryMethod.Cost;
            _context.Set<Factor>().Update(factor);
            await _context.SaveChangesAsync();
        }

        public async Task<Factor?> GetOpenFactorByUserIdAsync(int userId)
        {
            return await _context.Factors
                .Include(f => f.FactorDetails) // اگه نیاز داری جزئیات فاکتور رو هم بیاره
                .FirstOrDefaultAsync(f => f.UserId == userId
                                          && !f.IsFinaly
                                          && !f.IsDelete);
        }


        public async Task<FactorSummaryForSendSMS> GetFactorDteailForSendSMSAsync(int factorId)
        {
            var factor = await GetQuery()
                .Include(f=>f.User)
                .Include(f => f.FactorDetails)
                    .ThenInclude(fd => fd.Product)

                .Where(f => f.Id == factorId
                            && f.IsFinaly
                            && !f.IsDelete)
                .FirstOrDefaultAsync();

            if (factor == null)
                return null;

            var result = new FactorSummaryForSendSMS
            {
                FullName = factor.User?.FirstName + "" + factor.User?.LastName ?? "بدون نام",
                FactorId = factor.Id,
                Details = factor.FactorDetails
                    .Where(fd => !fd.IsDelete)
                    .Select(fd => new FactorDetailItemForSendSMS
                    {
                        ProductName = fd.Product.Title,
                        Count = fd.Count,

                    }).ToList()
            };
            return result;
        }

        public async Task<int> CountFactors()
        {
            return await _context.Factors
                .Where(o => o.IsFinaly)
                .CountAsync();
        }
        public async Task<decimal> GetTotalFactorsPriceAsync()
        {
            return await _context.Factors
              .Where(o => o.IsFinaly && !o.IsDelete)
              .SumAsync(o => o.TotalAmount);
        }

        public async Task<decimal> GetTodayTotalFactorsPriceAsync()
        {
            return await _context.Factors
              .Where(o => o.IsFinaly && !o.IsDelete && o.CreateDate.Date == DateTime.Now.Date)
              .SumAsync(o => o.TotalAmount);
        }
        public async Task<int> GetLastFactorId(int userId)
        {
            var factorId = await GetQuery()
                     .Where(f => f.UserId == userId && !f.IsFinaly && !f.IsDelete)
                     .OrderByDescending(f => f.Id) // آخرین فاکتور
                     .Select(f => f.Id)
                     .FirstOrDefaultAsync();
            return factorId;
        }

    }
}

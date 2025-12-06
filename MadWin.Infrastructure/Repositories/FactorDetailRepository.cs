using MadWin.Core.DTOs.Factors;
using MadWin.Core.Entities.Factors;
using MadWin.Core.Entities.Orders;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Context;
using MadWin.Infrastructure.Convertors;
using Microsoft.EntityFrameworkCore;
using Shop2City.Core.Convertors;


namespace MadWin.Infrastructure.Repositories
{
    public class FactorDetailRepository : GenericRepository<FactorDetail>, IFactorDetailRepository
    {
        private readonly MadWinDBContext _context;
        private readonly IProductRepository _productRepository;
       

        public FactorDetailRepository(MadWinDBContext context, IProductRepository productRepository) : base(context)
        {
            _context = context;
            _productRepository = productRepository;
           
        }

        public async Task<FactorDetail> AddFactorDetailAsync(int factorId, int count, int productId)
        {
            var product = await _productRepository.GetProductInfoByProductId(productId);
            FactorDetail factorDetail = new FactorDetail();

            factorDetail.FactorId = factorId;
            factorDetail.ProductId = productId;
            factorDetail.Count = count;
            factorDetail.Price =product.Price;
            factorDetail.IsNew=true;
            await AddAsync(factorDetail);
            await _context.SaveChangesAsync();
            return factorDetail;
        }

        public async Task<decimal> FactorSum(int factorId)
        {
            return await _context.Set<FactorDetail>()
                .Where(d => d.FactorId == factorId & !d.IsDelete)
                .SumAsync(d => d.Price * d.Count);
        }
        public async Task<bool> IsExistFactorDetailAsync(int factorId)
        {
            return await _context.Set<FactorDetail>().AsNoTracking().AnyAsync(d => d.FactorId == factorId);
        }


        public async Task<FactorDetail> GetFactorByfactorIdAsync(int factorId)
        {
            var factorDetail = await _context.Set<FactorDetail>()
                 .FirstOrDefaultAsync(d => d.FactorId == factorId);
            return factorDetail;
        }


        public async Task<FactorSummaryForAdminDto> GetOpenFactorAsync(int userId, int? factorId = null)
        {
  
            var query = GetQuery()
                .Include(f => f.Factor)
                    .ThenInclude(f => f.FactorDetails)
                        .ThenInclude(fd => fd.Product)
                .Where(fd => fd.FactorId == factorId
                             && fd.Factor.UserId == userId
                             && !fd.Factor.IsFinaly
                             && !fd.IsDelete
                             && !fd.Factor.IsDelete);

            var list = new FactorSummaryForAdminDto
            {
                FactorSummary = await query
                    .OrderByDescending(o => o.Id)
                    .Select(o => new FactorSummaryForAdminItemDto
                    {
                        FactorId = o.Factor.Id,
                        CreateDate = o.CreateDate,
                        SubTotal = o.Factor.SubTotal,

                        FactorDetails = o.Factor.FactorDetails
                            .Where(fd => !fd.IsDelete)
                            .Select(fd => new FactorDetailDto
                            {
                                Id = fd.Id,
                                ProductTitle = fd.Product.Title,
                                Count = fd.Count,
                                Price = fd.Price,
                                TotalPrice = fd.Price * fd.Count,
                                IsNew = fd.IsNew
                            }).ToList()
                    }).ToListAsync(),

                TotalCost = await query
                    .Select(o => o.Factor.FactorDetails
                        .Where(fd => fd.FactorId == factorId
                             && fd.Factor.UserId == userId
                             && !fd.Factor.IsFinaly
                             && !fd.IsDelete
                             && !fd.Factor.IsDelete)
                        .Sum(fd => fd.Price * fd.Count))
                    .FirstOrDefaultAsync()
            };

            return list;
        }

        public async Task<List<FactorDetail>> GetAllFactorDetailByFactorIdAsync(int factorId)
        {
            return await _context.FactorDetails
                .Include(od => od.Product)
                .Include(od => od.Factor)
                .ThenInclude(od => od.User)
                .Where(od => od.Factor.Id == factorId && od.Factor.IsFinaly)
                .ToListAsync();
        }

        public async Task<FactorDetail?> GetFactorDetailByProductIdAsync(int factorId, int productId)
        {
            return await _context.FactorDetails
                .FirstOrDefaultAsync(fd => fd.FactorId == factorId && fd.ProductId == productId && !fd.IsDelete);
        }

        public decimal GetSubtotalByFactorId(int factorId)
        {
            return _context.FactorDetails
                .Where(fd => fd.FactorId == factorId && !fd.IsDelete)
                .Sum(fd => (decimal?)fd.Price * fd.Count) ?? 0;
        }


        public async Task<FactorForAdminViewModel> GetAllFactorAsync(FactorFilterParameter filter, int pageId = 1)
        {
            DateTime? FromDate = string.IsNullOrWhiteSpace(filter.FromDate)
    ? null
    : DateConvertor.ConvertPersianToGregorian(filter.FromDate);

            DateTime? ToDate = string.IsNullOrWhiteSpace(filter.ToDate)
                ? null
                : DateConvertor.ConvertPersianToGregorian(filter.ToDate);


            IQueryable<Factor> query = _context.Factors
                .Include(f => f.User)
                .Include(f => f.DeliveryMethod)
                .Include(f => f.FactorDetails)
                    .ThenInclude(fd => fd.Product)
                .Where(f => !f.IsDelete);



            // فیلترها
            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                query = query.Where(f =>
                    (f.User.FirstName + " " + f.User.LastName).Contains(filter.FullName));
            }

            if (filter.FactorId.HasValue)
                query = query.Where(f => f.Id == filter.FactorId.Value);

            if (FromDate.HasValue)
                query = query.Where(f => f.CreateDate >= FromDate);

            if (ToDate.HasValue)
                query = query.Where(f => f.CreateDate <= ToDate);

            if (filter.FromPrice.HasValue)
                query = query.Where(f => f.TotalAmount >= filter.FromPrice.Value);

            if (filter.ToPrice.HasValue)
                query = query.Where(f => f.TotalAmount <= filter.ToPrice.Value);

            // Paging
            int take = 12;
            int skip = (pageId - 1) * take;

            // گرفتن تعداد صفحات قبل از Skip/Take
            int totalCount = await query.CountAsync();

            var factorList = await query
                .Skip(skip)
                .Take(take)
                .Select(f => new FactorSummaryForAdminItemDto
                {
                    FactorId = f.Id,
                    IsFinaly = f.IsFinaly,
                    FullName = f.User != null
                        ? $"{f.User.FirstName ?? ""} {f.User.LastName ?? ""}"
                        : "نامشخص",
                    CellPhone = f.User.CellPhone ?? "",
                    Address = f.User.Address ?? "",
                    CreateDate = f.CreateDate,
                    DeliveryMethodName=f.DeliveryMethod.Name,
                    DeliveryMethodCost = f.DeliveryMethodAmount,
                    SubTotal=f.SubTotal,
                    DisTotal = f.DisTotal,
                    Discount = f.DisTotal,
                    TotalAmount=f.TotalAmount,
                    Description=f.Description,
                    FactorDetailItemCount = f.FactorDetails.Count(fd => !fd.IsDelete),
                    FactorDetails = f.FactorDetails
                        .Where(fd => !fd.IsDelete)
                        .Select(fd => new FactorDetailDto
                        {
                            Id = fd.Id,
                            ProductTitle = fd.Product != null ? fd.Product.Title : "",
                            Count = fd.Count,
                            Price = fd.Price
                        }).ToList()
                })
                .ToListAsync();

            var result = new FactorForAdminViewModel
            {
                CurrentPage = pageId,
                CountPage = (int)Math.Ceiling(totalCount / (double)take),
                FactorSummary = factorList
            };

            return result;
        }

        public async Task<FactorForUserViewModel> GetAllFactorByUserIdAsync(int userId, FactorFilterParameter filter, int pageId = 1)
        {
            // تاریخ‌ها
            DateTime? FromDate = string.IsNullOrWhiteSpace(filter.FromDate)
                ? null
                : DateConvertor.ConvertPersianToGregorian(filter.FromDate);

            DateTime? ToDate = string.IsNullOrWhiteSpace(filter.ToDate)
                ? null
                : DateConvertor.ConvertPersianToGregorian(filter.ToDate);

            // شروع Query — بدون Include (ابتدا تست داده)
            IQueryable<Factor> query = _context.Factors
                .Where(f => !f.IsDelete && f.UserId == userId);  // شرط صحیح

            // فیلترها
            if (filter.FactorId.HasValue)
                query = query.Where(f => f.Id == filter.FactorId.Value);

            if (FromDate.HasValue)
                query = query.Where(f => f.CreateDate >= FromDate);

            if (ToDate.HasValue)
                query = query.Where(f => f.CreateDate <= ToDate);

            if (filter.FromPrice.HasValue)
                query = query.Where(f => f.TotalAmount >= filter.FromPrice.Value);

            if (filter.ToPrice.HasValue)
                query = query.Where(f => f.TotalAmount <= filter.ToPrice.Value);

            // شمردن تعداد رکوردها
            int take = 12;
            int skip = (pageId - 1) * take;
            int totalCount = await query.CountAsync();

            // حالا Include ایمن اضافه می‌کنیم
            query = query
                .Include(f => f.User)
                .Include(f => f.DeliveryMethod)  // ایمن — اگر NULL باشد مشکلی ندارد
                .Include(f => f.FactorDetails)
                    .ThenInclude(fd => fd.Product);

            // گرفتن لیست
            var factorList = await query
                .OrderByDescending(f => f.Id)
                .Skip(skip)
                .Take(take)
                .Select(f => new FactorSummaryForUserItemDto
                {
                    FactorId = f.Id,
                    IsFinaly = f.IsFinaly,
                    CellPhone = f.User.CellPhone ?? "",
                    Address = f.User.Address ?? "",
                    CreateDate = f.CreateDate,
<<<<<<< HEAD

                    DeliveryMethodName = f.DeliveryMethod != null ? f.DeliveryMethod.Name : "",
                    DeliveryMethodCost = f.DeliveryMethodAmount,

=======
                    DeliveryMethodName=f.DeliveryMethod.Name,
                    DeliveryMethodCost = f.DeliveryMethodAmount,
>>>>>>> f99be209bd00a959536bd2503c41a4c308b467b4
                    DisTotal = f.DisTotal,
                    Discount = f.DisTotal,
                    TotalAmount = f.TotalAmount,
                    Description = f.Description ?? "",

                    FactorDetailItemCount = f.FactorDetails.Count(fd => !fd.IsDelete),

                    FactorDetails = f.FactorDetails
                        .Where(fd => !fd.IsDelete)
                        .Select(fd => new FactorDetailForUserDto
                        {
                            FactorDetailId = fd.Id,
                            ProductTitle = fd.Product != null ? fd.Product.Title : "",
                            Count = fd.Count,
                            Price = fd.Price
                        }).ToList()
                })
                .ToListAsync();

            // مدل خروجی
            return new FactorForUserViewModel
            {
                CurrentPage = pageId,
                CountPage = (int)Math.Ceiling(totalCount / (double)take),
                FactorSummaryForUser = factorList
            };
        }

        public async Task<IEnumerable<FactorDetailDto>> GetByFactorIdAsync(int factorId)
        {
            return await _context.FactorDetails
                .Where(f => f.FactorId == factorId && !f.IsDelete)
                .Select(fd => new FactorDetailDto
                {
                    Id = fd.Id,
                    Price = fd.Price,
                    ProductTitle = fd.Product.Title,
                    Count = fd.Count,
                })
                .ToListAsync();

        }


   

    }
}
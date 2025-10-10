using MadWin.Core.DTOs.Fators;
using MadWin.Core.DTOs.FilterParameters;
using MadWin.Core.Entities.Factors;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


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
            factorDetail.Quantity = count;
            factorDetail.Price = count * product.Price;
            await AddAsync(factorDetail);
            await _context.SaveChangesAsync();
            return factorDetail;
        }

        public async Task<decimal> FactorSum(int factorId)
        {
            return await _context.Set<FactorDetail>().Where(d => d.FactorId == factorId).SumAsync(d => d.Price);
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


        public async Task<FactorSummaryForAdminDto> GetOpenFactorAsync(int userId, int factorId)
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
                        SubTotal = o.Factor.TotalPrice,

                        FactorDetails = o.Factor.FactorDetails
                            .Where(fd => !fd.IsDelete)
                            .Select(fd => new FactorDetailDto
                            {
                                Id = fd.Id,
                                ProductTitle = fd.Product.Title,
                                Quantity = fd.Quantity,
                                Price = fd.Price,
                                TotalPrice = fd.Price * fd.Quantity
                            }).ToList()
                    }).ToListAsync(),

                TotalCost = await query
                    .Select(o => o.Factor.FactorDetails
                        .Where(fd => fd.FactorId == factorId
                             && fd.Factor.UserId == userId
                             && !fd.Factor.IsFinaly
                             && !fd.IsDelete
                             && !fd.Factor.IsDelete)
                        .Sum(fd => fd.Price * fd.Quantity))
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
                .Sum(fd => (decimal?)fd.Price * fd.Quantity) ?? 0;
        }


        public async Task<FactorForAdminViewModel> GetAllFactorAsync(FilterParameter filter, int pageId = 1)
        {
            // کوئری اصلی (فقط سفارش‌های همین کاربر)
            IQueryable<FactorDetail> query = GetQuery()
                .Include(f => f.Factor)
                .ThenInclude(f => f.User)
                .Where(o => !o.IsDelete);

            // فیلترها
            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                query = query.Where(f =>
                    string.Concat(f.Factor.User.FirstName, " ", f.Factor.User.LastName).Contains(filter.FullName));
            }

            if (filter.OrderId.HasValue)
            {
                query = query.Where(f => f.Factor.Id == filter.OrderId.Value);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(f => f.Factor.CreateDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(f => f.Factor.CreateDate <= filter.ToDate.Value);
            }

            if (filter.FromPrice.HasValue)
            {
                query = query.Where(f => f.Factor.TotalAmount >= filter.FromPrice.Value);
            }

            if (filter.ToPrice.HasValue)
            {
                query = query.Where(f => f.Factor.TotalAmount <= filter.ToPrice.Value);
            }

            int take = 12;
            int skip = (pageId - 1) * take;

            var list = new FactorForAdminViewModel
            {
                CurrentPage = pageId,
                CountPage = (int)Math.Ceiling(query.Count() / (double)take),
                FactorSummary = await query
                    .OrderByDescending(u => u.FactorId)
                    .Skip(skip)
                    .Take(take)
                    .Select(f => new FactorSummaryForAdminItemDto
                    {
                        FactorId = f.Factor.Id,
                        IsFinaly = f.Factor.IsFinaly,
                        FullName = (f.Factor.User != null
                            ? (f.Factor.User.FirstName ?? "") + " " + (f.Factor.User.LastName ?? "")
                            : "نامشخص"),
                        CellPhone=f.Factor.User.CellPhone,
                        CreateDate = f.Factor.CreateDate,
                        DeliveryPrice = f.Factor.DeliveryMethodAmount, // قیمت ارسال
                        Discount = f.Factor.DisTotal, // مبلغ تخفیف
                        FactorDetailItemCount = f.Factor.FactorDetails
                            .Where(fd => !fd.IsDelete) // تعداد جزئیات فاکتور غیر حذف شده
                            .Count(), // محاسبه تعداد زیر فاکتورها
                        FactorDetails = f.Factor.FactorDetails
                            .Where(fd => !fd.IsDelete) // شرط روی جزئیات فاکتور
                            .Select(fd => new FactorDetailDto
                            {
                                Id = fd.Id,
                                ProductTitle = fd.Product.Title,
                                Quantity = fd.Quantity,
                                Price = fd.Price
                            }).ToList()
                    }).ToListAsync()
            };

            return list;
        }

        public async Task<FactorForAdminViewModel> GetAllFactorByUserIdAsync(int userId,FilterParameter filter, int pageId = 1)
        {
            // کوئری اصلی (فقط سفارش‌های همین کاربر)
            IQueryable<FactorDetail> query = GetQuery()
                .Include(f => f.Factor)
                .ThenInclude(f => f.User)
                .Where(fd => !fd.IsDelete && fd.Factor.UserId==userId);

            // فیلترها
            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                query = query.Where(f =>
                    string.Concat(f.Factor.User.FirstName, " ", f.Factor.User.LastName).Contains(filter.FullName));
            }

            if (filter.OrderId.HasValue)
            {
                query = query.Where(f => f.Factor.Id == filter.OrderId.Value);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(f => f.Factor.CreateDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(f => f.Factor.CreateDate <= filter.ToDate.Value);
            }

            if (filter.FromPrice.HasValue)
            {
                query = query.Where(f => f.Factor.TotalAmount >= filter.FromPrice.Value);
            }

            if (filter.ToPrice.HasValue)
            {
                query = query.Where(f => f.Factor.TotalAmount <= filter.ToPrice.Value);
            }

            int take = 12;
            int skip = (pageId - 1) * take;

            var list = new FactorForAdminViewModel
            {
                CurrentPage = pageId,
                CountPage = (int)Math.Ceiling(query.Count() / (double)take),
                FactorSummary = await query
                    .OrderByDescending(u => u.FactorId)
                    .Skip(skip)
                    .Take(take)
                    .Select(f => new FactorSummaryForAdminItemDto
                    {
                        FactorId = f.Factor.Id,
                        IsFinaly = f.Factor.IsFinaly,
                        FullName = (f.Factor.User != null
                            ? (f.Factor.User.FirstName ?? "") + " " + (f.Factor.User.LastName ?? "")
                            : "نامشخص"),
                        CreateDate = f.Factor.CreateDate,
                        DeliveryPrice = f.Factor.DeliveryMethodAmount, // قیمت ارسال
                        Discount = f.Factor.DisTotal, // مبلغ تخفیف
                        FactorDetailItemCount = f.Factor.FactorDetails
                            .Where(fd => !fd.IsDelete) // تعداد جزئیات فاکتور غیر حذف شده
                            .Count(), // محاسبه تعداد زیر فاکتورها
                        FactorDetails = f.Factor.FactorDetails
                            .Where(fd => !fd.IsDelete) // شرط روی جزئیات فاکتور
                            .Select(fd => new FactorDetailDto
                            {
                                Id = fd.Id,
                                ProductTitle = fd.Product.Title,
                                Quantity = fd.Quantity,
                                Price = fd.Price
                            }).ToList()
                    }).ToListAsync()
            };

            return list;
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
                    Quantity = fd.Quantity,
                })
                .ToListAsync();

        }

       


    }
}
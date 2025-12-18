

using MadWin.Core.DTOs.Products;
using MadWin.Core.Entities.Products;
using MadWin.Core.Interfaces;
using MadWin.Core.Lookups.Products;
using MadWin.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MadWin.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly MadWinDBContext _context;
        public ProductRepository(MadWinDBContext context):base(context)
        {
            _context=context;
        }

        public async Task<ProductForAdminViewModel> GetAllProducts(int pageId = 1, string filterTitleProduct = "")
        {
            IQueryable<Product> result = GetQuery()
                .IgnoreQueryFilters()
                .Where(p => !p.IsDelete)
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(p=>p.FactorDetails);

            if (!string.IsNullOrEmpty(filterTitleProduct))
            {
                result = result.Where(u => u.Title.Contains(filterTitleProduct));
            }

            int take = 10;
            int skip = (pageId - 1) * take;

            var list = new ProductForAdminViewModel
            {
                CurrentPage = pageId,
                CountPage = (int)Math.Ceiling(result.Count() / (double)take),
                Products = await result
                    .OrderBy(u => u.Id)
                    .Skip(skip)
                    .Take(take)
                    .Select(u => new ProductForAdminItemViewModel
                    {
                        ProductId = u.Id,
                        Title = u.Title,
                        Price = u.Price,
                        LastUpdateDate=u.LastUpdateDate,
                        IsStatus=u.IsStatus,
                        FactorCount=u.FactorDetails.Where(fd=>fd.ProductId==u.Id).Count()
                    })
                    .ToListAsync()
            };

            return list;
        }

        public async Task<ProductInfoLookup> GetProductInfoByProductId(int productId)
        {
            var item = await _context.Set<Product>()
         .AsNoTracking()
         .Include(p=>p.ProductGalleries)
         .FirstOrDefaultAsync(u => u.Id == productId);

            if (item == null)
                return null;

            return new ProductInfoLookup
            {
                ProductId=item.Id,
                Title=item.Title,
                Price=item.Price,
                ImageName= item.ProductGalleries.FirstOrDefault().FileName ?? ""
            };
        }

        public async Task<IEnumerable<ShowProductListItemViewModel>> GetAllProductsByGroupId(int groupId)
        {
            var items = await _context.Set<Product>()
                .AsNoTracking()
                .Include(p => p.ProductGalleries)
                .Where(p => p.SubCategoryId == groupId)
                .Select(p => new ShowProductListItemViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    IsStatus = p.IsStatus,
                    ShortDescription = p.ShortDescription,
                    FileName = p.ProductGalleries.FirstOrDefault().FileName ?? ""
                })
                .ToListAsync();

            return items;
        }

        public async Task<List<SelectListItem>> GetCategoryForManageProduct(int groupId)
        {
            var items = await _context.Set<ProductGroup>()
                .AsNoTracking()
                .Where(pg => pg.ParentId == groupId && pg.IsActive)
                .OrderBy(pg => pg.Title)
                .ToListAsync();

            return items.Select(pg => new SelectListItem
            {
                Text = pg.Title,
                Value = pg.Id.ToString()
            }).ToList();
        }

        public async Task<List<SelectListItem>> GetSubCategoryForManageProduct(int categoryId)
        {
            var items = await _context.Set<ProductGroup>()
                .AsNoTracking()
                .Where(pg => pg.ParentId == categoryId)
                .OrderBy(pg => pg.Title)
                .ToListAsync();

            return items.Select(pg => new SelectListItem
            {
                Text = pg.Title,
                Value = pg.Id.ToString()
    }).ToList();
}
    }
}

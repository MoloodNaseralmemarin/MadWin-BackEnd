using MadWin.Core.DTOs.ProductGroups;
using MadWin.Core.Entities.Products;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace MadWin.Infrastructure.Repositories
{
    public class ProductGroupRepository : GenericRepository<ProductGroup>, IProductGroupRepository
    {
        private readonly MadWinDBContext _context;
        public ProductGroupRepository(MadWinDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProductGroupForAdminDto> GetAllProductGroupsAsync(int pageId = 1, string filterTitle = "")
        {
            IQueryable<ProductGroup> result = GetQuery()
                .IgnoreQueryFilters()
                .Where(pg => !pg.IsDelete);

            if (!string.IsNullOrEmpty(filterTitle))
            {
                result = result.Where(u => u.Title.Contains(filterTitle));
            }

            int take = 10;
            int skip = (pageId - 1) * take;

            var list = new ProductGroupForAdminDto
            {
                CurrentPage = pageId,
                CountPage = (int)Math.Ceiling(result.Count() / (double)take),
                ProductGroups = await result
                    .OrderBy(pg => pg.Id)
                    .Skip(skip)
                    .Take(take)
                    .Select(pg => new ProductGroupItemForAdminDto
                    {
                        Id = pg.Id,
                        Title=pg.Title,
                        ParentId = pg.ParentId
                    })
                    .ToListAsync()
            };
            return list;
        }

    }
}

using MadWin.Core.DTOs.Calculations;
using MadWin.Core.Entities.CurtainComponents;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace MadWin.Infrastructure.Repositories
{
    public class CurtainComponentProductGroupRepository : GenericRepository<CurtainComponentProductGroup>, ICurtainComponentProductGroupRepository
    {
        private readonly MadWinDBContext _context;
        public CurtainComponentProductGroupRepository(MadWinDBContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CurtainComponentProductGroupLookup>> CalculationByCategory(int categoryId, int subCategoryId)
        {
            return await _context.CurtainComponentProductGroups
                    .Where(c => c.CategoryId == categoryId
                                && c.SubCategoryId == subCategoryId)
                    .Select(c => new CurtainComponentProductGroupLookup
                    {
                        CategoryId = c.CategoryId,
                        SubCategoryId=c.SubCategoryId,
                        CurtainComponentId = c.CurtainComponentId
                    })
                    .ToListAsync();
        }
    }
}


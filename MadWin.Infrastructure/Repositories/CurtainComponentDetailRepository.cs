using MadWin.Core.DTOs.Calculations;
using MadWin.Core.Entities.CurtainComponents;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Infrastructure.Repositories
{
    public class CurtainComponentDetailRepository:GenericRepository<CurtainComponentDetail>, ICurtainComponentDetailRepository
    {
        private readonly MadWinDBContext _context;
        public CurtainComponentDetailRepository(MadWinDBContext context):base(context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<CurtainComponentDetailDto>> GetCurtainComponentDetailByOrderIdAsync(int orderId)
        {
            return await _context.CurtainComponentDetails
                .Include(cd => cd.CurtainComponent)
                .Include(cd => cd.Order)
                    .ThenInclude(o => o.OrderCategory)
                .Include(cd => cd.Order)
                    .ThenInclude(o => o.OrderSubCategory)
                .Where(cd => cd.OrderId == orderId)
                .Select(cd => new CurtainComponentDetailDto
                {
                    Id = cd.Id,
                    OrderId = cd.OrderId,
                    CurtainComponentId = cd.CurtainComponentId,
                    ComponentName = cd.CurtainComponent.Name,
                    UnitCost = cd.UnitCost,
                    Count = cd.Count,
                    FinalCost = cd.FinalCost,

                    Width = cd.Order.Width,
                    Height = cd.Order.Height,
                    BasePrice = cd.Order.BasePrice,
                    TotalAmount = cd.Order.TotalAmount,
                    CategoryName =cd.Order.OrderCategory.Title +"/"+cd.Order.OrderSubCategory.Title
                })
                .ToListAsync();
        }

    }
}

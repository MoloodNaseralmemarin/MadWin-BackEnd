using MadWin.Application.Services;
using MadWin.Core.Entities.CommissionRates;
using MadWin.Core.Entities.Products;
using MadWin.Core.Interfaces;
using MadWin.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Infrastructure.Repositories
{
    public class ProductGroupRepository : GenericRepository<ProductGroup>, IProductGroupRepository
    {
        private readonly MadWinDBContext _context;
        public ProductGroupRepository(MadWinDBContext context) : base(context)
        {
            _context = context;
        }
    }
}

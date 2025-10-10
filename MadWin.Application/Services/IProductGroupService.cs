using MadWin.Core.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Application.Services
{
    public interface IProductGroupService
    {
        Task<IEnumerable<ProductGroup>> AllProductGroupsAsync();
    }
}

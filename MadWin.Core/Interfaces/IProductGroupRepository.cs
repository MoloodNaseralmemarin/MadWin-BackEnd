using MadWin.Core.DTOs.ProductGroups;
using MadWin.Core.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Core.Interfaces
{
    public interface IProductGroupRepository:IGenericRepository<ProductGroup>
    {
        Task<ProductGroupForAdminDto> GetAllProductGroupsAsync(int pageId = 1, string filterTitle = "");
    }
}

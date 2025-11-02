using MadWin.Core.DTOs.ProductGroups;
using MadWin.Core.Entities.Products;

namespace MadWin.Application.Services
{
    public interface IProductGroupService
    {
        Task<IEnumerable<ProductGroup>> AllProductGroupsAsync();
        Task<ProductGroupForAdminDto> GetAllProductGroupsAsync(int pageId = 1, string filterTitle = "");
        Task<ProductGroup> GetByIdAsync(int id);
        Task<bool> EditProductGroupsAsync(ProductGroup productGroup);
    }
}

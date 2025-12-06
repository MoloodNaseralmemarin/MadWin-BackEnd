using MadWin.Core.DTOs.Products;
using MadWin.Core.Entities.Products;
using MadWin.Core.Lookups.Products;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MadWin.Core.Interfaces
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        Task<ProductInfoLookup> GetProductInfoByProductId(int productId);

        Task<ProductForAdminViewModel> GetAllProducts(int pageId = 1, string filterTitleProduct = "");

        Task<IEnumerable<ShowProductListItemViewModel>> GetAllProductsByGroupId(int groupId);

        Task<List<SelectListItem>> GetCategoryForManageProduct(int groupId);

        Task<List<SelectListItem>> GetSubCategoryForManageProduct(int categoryId);
    }
}

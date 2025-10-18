using MadWin.Core.DTOs.Products;
using MadWin.Core.DTOs.Users;
using MadWin.Core.Entities.Products;
using MadWin.Core.Entities.Properties;
using MadWin.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shop2City.Core.Services.Products
{
    public interface IProductService
    {
        Task<ProductForAdminViewModel> GetAllProducts(int pageId = 1, string filterTitleProduct = "");

        Task<IEnumerable<ShowProductListItemViewModel>> GetAllProductsByGroupId(int groupId);

        Task<List<SelectListItem>> GetCategoryForManageProduct(int groupId);
        Task<List<SelectListItem>> GetSubCategoryForManageProduct(int categoryId);

    }
}

     













       



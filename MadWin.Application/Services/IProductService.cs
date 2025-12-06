using MadWin.Application.DTOs.Products;
using MadWin.Core.DTOs.Products;
using MadWin.Core.Entities.CommissionRates;
using MadWin.Core.Entities.Products;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shop2City.Core.Services.Products
{
    public interface IProductService
    {
        Task<ProductForAdminViewModel> GetAllProducts(int pageId = 1, string filterTitleProduct = "");

        Task<IEnumerable<ShowProductListItemViewModel>> GetAllProductsByGroupId(int groupId);

        Task<List<SelectListItem>> GetCategoryForManageProduct(int groupId);
        Task<List<SelectListItem>> GetSubCategoryForManageProduct(int categoryId);


       Task<EditProductDto> GetProductByIdAsync(int id);

        Task<bool> EditProductAsync(Product editProduct);

        Task<bool> EditIsStatusProdct(bool isStatus,int productId);

    }
}

     













       



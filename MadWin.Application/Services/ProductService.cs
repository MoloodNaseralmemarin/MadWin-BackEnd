using MadWin.Core.DTOs.Products;
using MadWin.Core.Entities.Products;
using MadWin.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shop2City.Core.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        public async Task<ProductForAdminViewModel> GetAllProducts(int pageId = 1, string filterTitleProduct = "")
        {
            return await _productRepository.GetAllProducts(pageId, filterTitleProduct);
        }

        public async Task<IEnumerable<ShowProductListItemViewModel>> GetAllProductsByGroupId(int groupId)
        {
            return await _productRepository.GetAllProductsByGroupId(groupId);
        }

        public async Task<List<SelectListItem>> GetCategoryForManageProduct(int groupId)
        {
            return await _productRepository.GetCategoryForManageProduct(groupId);
        }
        public async Task<List<SelectListItem>> GetSubCategoryForManageProduct(int categoryId)
        {
            return await _productRepository.GetSubCategoryForManageProduct(categoryId);
        }
    }
}

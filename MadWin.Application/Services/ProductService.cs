﻿using MadWin.Application.DTOs.Products;
using MadWin.Core.DTOs.Products;
using MadWin.Core.Entities.CommissionRates;
using MadWin.Core.Entities.CurtainComponents;
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

     
        public async Task<bool> EditProductAsync(EditProductDto editProduct)
        {
            var existing = await _productRepository.GetByIdAsync(editProduct.ProductId);
            if (existing == null) return false;

            existing.Title = editProduct.Title;
            existing.Price = editProduct.Price;
            existing.LastUpdateDate = DateTime.UtcNow;

            await _productRepository.SaveChangesAsync();
            return true;
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

        public async Task<EditProductDto> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                var productDto = new EditProductDto
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    Price = product.Price
                };

                return productDto;
            }
            catch (Exception ex)
            {
                // مدیریت استثناها
                throw new Exception("به مشکل خورد", ex);
            }
        }


        public async Task<List<SelectListItem>> GetSubCategoryForManageProduct(int categoryId)
        {
            return await _productRepository.GetSubCategoryForManageProduct(categoryId);
        }
    }
}

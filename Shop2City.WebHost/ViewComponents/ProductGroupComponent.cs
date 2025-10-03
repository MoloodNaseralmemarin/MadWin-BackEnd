using System.Threading.Tasks;
using MadWin.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Shop2City.Core.Services.Products;

namespace Shop2City.Web.ViewComponents
{
    public class ProductGroupComponent:ViewComponent
    {
        private readonly IProductGroupService _productGroupService;

        public ProductGroupComponent(IProductGroupService productGroupService)
        {
            _productGroupService = productGroupService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allGroup = await _productGroupService.AllProductGroupsAsync();
            if (!allGroup.Any())
                return Content("");
            return await Task.FromResult((IViewComponentResult)View("ProductGroup", allGroup));
        }
    }
}

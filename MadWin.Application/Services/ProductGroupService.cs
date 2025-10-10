using MadWin.Core.Entities.Products;
using MadWin.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadWin.Application.Services
{
    public class ProductGroupService:IProductGroupService
    {
        public IProductGroupRepository _productGroupRepository { get; set; }
        public ProductGroupService(IProductGroupRepository productGroupRepository)
        {
             _productGroupRepository = productGroupRepository;   
        }
        public async Task<IEnumerable<ProductGroup>> AllProductGroupsAsync()
        {
            return await _productGroupRepository.GetAllAsync();
        }
    }
}

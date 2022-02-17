using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;

namespace Interfaces
{
    public interface IProductGroupRepository : IRepository<ProductGroup>
    {
        Task<IQueryable<Product>> GetProductsAsync(int productGroupID);
    }
}
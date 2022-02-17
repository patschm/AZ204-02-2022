using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Repository.InMemory
{
    public class ProductGroupRepository : BaseRepository<ProductGroup>, IProductGroupRepository
    {
        public Task<IQueryable<Product>> GetProductsAsync(int productGroupID)
        {
            var query = Context.ProductGroupProducts
                .Include(p=>p.Product)
                .ThenInclude(b => b.Brand)
                .Where(p => p.ProductGroupID == productGroupID)
                .Select(p => p.Product);
            return Task.FromResult(query);
        }
    }
}
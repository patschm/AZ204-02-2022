using Contexts;
using Entities;
using Interfaces;

namespace Repository.InMemory
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ProductContext ctx) : base(ctx)
        {

        }
    }
}

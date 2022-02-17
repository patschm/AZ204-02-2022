using Contexts;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository.InMemory
{
    public class UnitOfWork : IUnitOfWork
    {
        private static ProductContext _context;
        protected static ProductContext Context
        {
            get
            {
                if (_context == null)
                {
                    var builder = new DbContextOptionsBuilder<ProductContext>();
                    builder.UseInMemoryDatabase("products");
                    _context = new ProductContext(builder.Options);
                    DbInitializer.InitializeDatabase(_context).Wait();
                }
                return _context;
            }
        }
        public IBrandRepository BrandRepository { get; } = new BrandRepository(Context);
        public IProductGroupRepository ProductGroupRepository { get; } = new ProductGroupRepository(Context);
        public IProductRepository ProductRepository { get; } = new ProductRepository(Context);

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}

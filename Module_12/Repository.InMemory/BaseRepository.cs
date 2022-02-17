using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Contexts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Repository.InMemory
{
    public class BaseRepository<T> where T: class 
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
        
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var dbi = await Context.Set<T>().FindAsync(id);
            if (dbi == null) return false;
            Context.Remove(dbi);
            var mods = await Context.SaveChangesAsync();
            return mods > 0;
        }

        public virtual async Task<IQueryable<T>> GetAllAsync(int start, int count)
        {
            return await Task.FromResult(Context.Set<T>().Skip(start).Take(count));
        }

        public virtual async Task<T> GetAsync(int id)
        {
           return await Context.Set<T>().FindAsync(id);
        }

        public virtual async Task<bool> InsertAsync(T item)
        {
           var result = await Context.Set<T>().AddAsync(item);
           var mods = await Context.SaveChangesAsync();
           return mods > 0;
        }

        public virtual async Task<bool> UpdateAsync(T item)
        {
            var result = Context.Set<T>().Update(item);
            var mods = await Context.SaveChangesAsync();
            return  mods > 0;
        }
    }
}
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
        private ProductContext _context;

        public BaseRepository(ProductContext ctx)
        {
            _context = ctx;
        }
        protected ProductContext Context
        {
            get
            {
                return _context;
            }
        }
        public virtual async Task DeleteAsync(int id)
        {
            var dbi = await Context.Set<T>().FindAsync(id);
            if (dbi == null) return;
            Context.Remove(dbi);
        }

        public virtual async Task<IQueryable<T>> GetAllAsync(int start, int count)
        {
            return await Task.FromResult(Context.Set<T>().Skip(start).Take(count));
        }

        public virtual async Task<T> GetAsync(int id)
        {
           return await Context.Set<T>().FindAsync(id);
        }

        public virtual async Task InsertAsync(T item)
        {
           var result = await Context.Set<T>().AddAsync(item);
        }

        public virtual async Task UpdateAsync(T item)
        {
            Context.Set<T>().Update(item);
            await Task.FromResult(0);
        }
    }
}
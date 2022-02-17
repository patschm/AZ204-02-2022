using System.Linq;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IRepository<T> where T: class
    {
        Task<IQueryable<T>> GetAllAsync(int start, int count);
        Task<T> GetAsync(int id);
        Task InsertAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(int id);
    }
}
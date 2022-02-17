using System.Linq;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IRepository<T> where T: class
    {
        Task<IQueryable<T>> GetAllAsync(int start, int count);
        Task<T> GetAsync(int id);
        Task<bool> InsertAsync(T item);
        Task<bool> UpdateAsync(T item);
        Task<bool> DeleteAsync(int id);
    }
}
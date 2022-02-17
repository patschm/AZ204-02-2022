using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IUnitOfWork
    {
        IBrandRepository BrandRepository { get; }
        IProductGroupRepository ProductGroupRepository { get; }
        IProductRepository ProductRepository { get; }

        Task SaveAsync();
    }
}


using Entity.BaseModel;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepoBase<T> where T : class
    {
        Task<List<T>> FindAll();
        Task<T> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        void Create(T entities);
        void Update(T entity);
        Task Delete(List<int> list);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
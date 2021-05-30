using Contracts;
using Entity.BaseModel;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class RepoBase<T> : IRepoBase<T> where T : class
    {
        protected readonly RepoContext _repoContext;
        protected readonly DbSet<T> _dbSet;
        public RepoBase(RepoContext repoContext)
        {
            _repoContext = repoContext;
            _dbSet = _repoContext.Set<T>();
        }
        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public void Create(T entities)
        {
            _dbSet.Add(entities);
        }

       

        public virtual async Task<List<T>> FindAll()
        {
            var items = await _dbSet.AsNoTracking().ToListAsync();
            return items;
        }

        public virtual async Task<T> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var items = await _dbSet.FindAsync(id);
            return items;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
           await _repoContext.SaveChangesAsync();
            
        }
        public async Task Delete(List<int> list)
        {
            foreach(int i in list)
            {
                var item = await _dbSet.FindAsync(i);
                _dbSet.Remove(item);
            }
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}

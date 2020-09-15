using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public enum OperationResult
    {
        Failed,
        Successed
    }

    interface IEntityService<TEntity, TId>
    {
        TEntity GetById(TId id);
        Task<OperationResult> AddAsync(TEntity entity);
        Task<OperationResult> RemoveAsync(TId id);
        Task<OperationResult> RemoveAsync(TEntity entity);
        Task<OperationResult> UpdateAsync(TId id, TEntity entity);
    }
}

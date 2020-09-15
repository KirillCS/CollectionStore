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

    interface IEntityManager<TEntity, TId>
    {
        TEntity GetById(TId id, bool coherentlyLoad);
        Task<OperationResult> AddAsync(TEntity entity);
        Task<OperationResult> RemoveAsync(TId id);
        Task<OperationResult> UpdateAsync(TId id, TEntity sourceEntity);
    }
}

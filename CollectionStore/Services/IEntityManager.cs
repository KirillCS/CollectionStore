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
        TEntity GetById(TId id, bool coherentlyLoad = false);
        Task<OperationResult> AddAsync(TEntity entity, bool saveChanges = true);
        Task<OperationResult> RemoveAsync(TId id, bool saveChanges = true);
        Task<OperationResult> UpdateAsync(TId id, TEntity sourceEntity, bool saveChanges = true);
    }
}

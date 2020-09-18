using CollectionStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public abstract class BaseEntityManager<TEntity, TId> : IEntityManager<TEntity, TId>
    {
        protected readonly ApplicationDbContext context;

        public BaseEntityManager(ApplicationDbContext context)
        {
            this.context = context;
        }

        public TEntity GetById(TId id, bool coherentlyLoad = false)
        {
            return coherentlyLoad ? GetByIdCoherentlyLoad(id) : GetById(id);
        }
        public async Task<OperationResult> AddAsync(TEntity entity, bool saveChanges = true)
        {
            if (entity == null)
            {
                return OperationResult.Failed;
            }
            try
            {
                await AddEntity(entity);
                if (saveChanges) await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return OperationResult.Failed;
            }
            return OperationResult.Successed;
        }
        public async Task<OperationResult> RemoveAsync(TId id, bool saveChanges = true)
        {
            var entity = GetByIdCoherentlyLoad(id);
            if (entity != null)
            {
                try
                {
                    RemoveEntity(entity);
                    if (saveChanges) await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return OperationResult.Failed;
                }
                return OperationResult.Successed;
            }
            return OperationResult.Failed;
        }
        public async Task<OperationResult> UpdateAsync(TId id, TEntity sourceEntity, bool saveChanges = true)
        {
            var entity = GetByIdCoherentlyLoad(id);
            if (sourceEntity == null || entity == null)
            {
                return OperationResult.Failed;
            }
            try
            {
                UpdateEntity(entity, sourceEntity);
                if (saveChanges) await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return OperationResult.Failed;
            }

            return OperationResult.Successed;
        }

        protected abstract TEntity GetById(TId id);
        protected abstract TEntity GetByIdCoherentlyLoad(TId id);
        protected abstract Task AddEntity(TEntity entity);
        protected abstract void RemoveEntity(TEntity entity);
        protected abstract void UpdateEntity(TEntity entity, TEntity sourceEntity);
    }
}

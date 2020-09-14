using CollectionStore.Data;
using CollectionStore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public class CollectionService : IEntityService<Collection, int>
    {
        private readonly ApplicationDbContext context;

        public CollectionService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Collection GetById(int id)
        {
            return context.Collections.FirstOrDefault(c => c.Id == id);
        }
        public async Task<OperationResult> AddAsync(Collection entity)
        {
            if (entity == null)
            {
                return OperationResult.Failed;
            }
            try
            {
                await context.Collections.AddAsync(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return OperationResult.Failed;
            }
            return OperationResult.Successed;
        }
        public async Task<OperationResult> RemoveAsync(int id)
        {
            var collection = context.Collections.FirstOrDefault(c => c.Id == id);
            if (collection != null)
            {
                return await RemoveAsync(collection);
            }
            return OperationResult.Failed;
        }
        public async Task<OperationResult> RemoveAsync(Collection entity)
        {
            if (entity == null)
            {
                return OperationResult.Failed;
            }
            try
            {
                RemoveCollection(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return OperationResult.Failed;
            }
            return OperationResult.Successed;
        }
        public async Task<OperationResult> UpdateAsync(Collection entity)
        {
            var collection = context.Collections.FirstOrDefault(c => c.Id == entity.Id);
            if (entity == null || collection == null)
            {
                return OperationResult.Failed;
            }
            SetCollection(collection, entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return OperationResult.Failed;
            }

            return OperationResult.Successed;
        }

        private void RemoveCollection(Collection collection)
        {
            RemoveItems(collection);
            RemoveFields(collection);
            context.Collections.Remove(collection);
        }
        private void RemoveItems(Collection collection)
        {
            foreach (var item in collection.Items)
            {
                context.FieldValues.RemoveRange(context.FieldValues.Where(fv => fv.ItemId == item.Id));
            }
            context.Items.RemoveRange(context.Items.Where(i => i.CollectionId == collection.Id));
        }
        private void RemoveFields(Collection collection)
        {
            context.Fields.RemoveRange(context.Fields.Where(f => f.CollectionId == collection.Id));
        }
        private void SetCollection(Collection collection, Collection entity)
        {
            collection.Name = entity.Name;
            collection.Description = entity.Description;
            collection.ImagePath = entity.ImagePath;
            collection.ThemeId = entity.ThemeId;
            collection.UserId = entity.UserId;
            collection.Fields = entity.Fields;
            collection.Items = entity.Items;
        }
    }
}

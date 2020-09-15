using CollectionStore.Data;
using CollectionStore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public class ItemService : IEntityService<Item, int>
    {
        private readonly ApplicationDbContext context;

        public ItemService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Item GetById(int id)
        {
            return context.Items.FirstOrDefault(i => i.Id == id);
        }
        public async Task<OperationResult> AddAsync(Item entity)
        {
            if (entity == null)
            {
                return OperationResult.Failed;
            }
            try
            {
                await context.Items.AddAsync(entity);
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
            var item = context.Items.FirstOrDefault(i => i.Id == id);
            if(item != null)
            {
                return await RemoveAsync(item);
            }
            return OperationResult.Failed;
        }
        public async Task<OperationResult> RemoveAsync(Item entity)
        {
            if(entity == null)
            {
                return OperationResult.Failed;
            }
            try
            {
                RemoveItem(entity);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return OperationResult.Failed;
            }
            return OperationResult.Successed;
        }
        public async Task<OperationResult> UpdateAsync(int id, Item entity)
        {
            var item = context.Items.FirstOrDefault(i => i.Id == id);
            if (entity == null || item == null)
            {
                return OperationResult.Failed;
            }
            try
            {
                SetItem(item, entity);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return OperationResult.Failed;
            }

            return OperationResult.Successed;
        }

        private void RemoveItem(Item item)
        {
            context.FieldValues.RemoveRange(context.FieldValues.Where(fv => fv.ItemId == item.Id));
            context.ItemTags.RemoveRange(context.ItemTags.Where(it => it.ItemId == item.Id));
            context.Items.Remove(item);
        }
        private void SetItem(Item item, Item sourceItem)
        {
            item.Name = sourceItem.Name;
            item.CollectionId = sourceItem.CollectionId;
            item.FieldValues = sourceItem.FieldValues;
            item.ItemTags = sourceItem.ItemTags;
        }
    }
}

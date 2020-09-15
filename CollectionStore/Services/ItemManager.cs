using CollectionStore.Data;
using CollectionStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public class ItemManager : BaseEntityManager<Item, int>
    {
        public ItemManager(ApplicationDbContext context) : base(context) { }

        protected override Item GetById(int id) => context.Items.FirstOrDefault(i => i.Id == id);
        protected override Item GetByIdCoherentlyLoad(int id)
        {
            var item = GetById(id);
            context.Entry(item).Reference(i => i.Collection).Load();
            context.Entry(item).Collection(i => i.FieldValues).Load();
            context.Entry(item).Collection(i => i.ItemTags).Load();
            return item;
        }
        protected async override Task AddEntity(Item entity)
        {
            await context.Items.AddAsync(entity);
        }
        protected override void RemoveEntity(Item entity)
        {
            RemoveFieldValues(entity);
            RemoveItemTags(entity);
            context.Items.Remove(entity);
        }
        protected override void UpdateEntity(Item entity, Item sourceEntity)
        {
            entity.Name = sourceEntity.Name;
            entity.CollectionId = sourceEntity.CollectionId;
            entity.FieldValues = sourceEntity.FieldValues;
            entity.ItemTags = sourceEntity.ItemTags;
        }

        private void RemoveFieldValues(Item entity)
        {
            context.FieldValues.RemoveRange(context.FieldValues.Where(fv => fv.ItemId == entity.Id));
        }
        private void RemoveItemTags(Item entity)
        {
            context.ItemTags.RemoveRange(context.ItemTags.Where(it => it.ItemId == entity.Id));
        }
    }
}

using CollectionStore.Data;
using CollectionStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public class CollectionManager : BaseEntityManager<Collection, int>
    {
        public CollectionManager(ApplicationDbContext context) : base(context) { }

        protected override Collection GetById(int id) => context.Collections.FirstOrDefault(c => c.Id == id);
        protected override Collection GetByIdCoherentlyLoad(int id)
        {
            return context.Collections.Where(c => c.Id == id)
                .Include(c => c.Theme)
                .Include(c => c.User)
                .Include(c => c.Fields)
                .ThenInclude(f => f.Type)
                .Include(c => c.Items)
                .ThenInclude(i => i.FieldValues)
                .Include(c => c.Items)
                .ThenInclude(i => i.ItemTags)
                .Include(c => c.Items)
                .ThenInclude(i => i.Comments)
                .FirstOrDefault(c => c.Id == id);
        }
        protected async override Task AddEntity(Collection entity)
        {
            await context.Collections.AddAsync(entity);
        }
        protected override void RemoveEntity(Collection entity)
        {
            RemoveItems(entity);
            RemoveFields(entity);
            context.Collections.Remove(entity);
        }
        protected override void UpdateEntity(Collection entity, Collection sourceEntity)
        {
            entity.Name = sourceEntity.Name;
            entity.Description = sourceEntity.Description;
            entity.ImagePath = sourceEntity.ImagePath;
            entity.ThemeId = sourceEntity.ThemeId;
            entity.UserId = sourceEntity.UserId;
            entity.Fields = sourceEntity.Fields;
            entity.Items = sourceEntity.Items;
        }

        private void RemoveItems(Collection entity)
        {
            foreach (var item in entity.Items)
            {
                context.FieldValues.RemoveRange(context.FieldValues.Where(fv => fv.ItemId == item.Id));
                context.ItemTags.RemoveRange(context.ItemTags.Where(it => it.ItemId == item.Id));
                context.Comments.RemoveRange(context.Comments.Where(c => c.ItemId == item.Id));
            }
            context.Items.RemoveRange(context.Items.Where(i => i.CollectionId == entity.Id));
        }
        private void RemoveFields(Collection entity)
        {
            context.Fields.RemoveRange(context.Fields.Where(f => f.CollectionId == entity.Id));
        }
    }
}

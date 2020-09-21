using CollectionStore.Data;
using CollectionStore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public class ItemManager : BaseEntityManager<Item, int>
    {
        private readonly TagManager tagManager;

        public ItemManager(ApplicationDbContext context, TagManager tagManager) : base(context) 
        {
            this.tagManager = tagManager;
        }

        public async Task AddTagAsync(string tagContent, int itemId, bool saveChanges = true)
        {
            if (context.Tags.FirstOrDefault(t => t.Content == tagContent) == null)
            {
                await tagManager.AddAsync(new Tag { Content = tagContent });
            }
            var tag = context.Tags.FirstOrDefault(t => t.Content == tagContent);
            await context.ItemTags.AddAsync(new ItemTag { ItemId = itemId, TagId = tag.Id });
            if (saveChanges) await context.SaveChangesAsync();
        }

        protected override Item GetById(int id) => context.Items.FirstOrDefault(i => i.Id == id);
        protected override Item GetByIdCoherentlyLoad(int id)
        {
            var item = GetById(id);
            context.Entry(item).Reference(i => i.Collection).Load();
            context.Entry(item).Collection(i => i.FieldValues).Load();
            context.Entry(item).Collection(i => i.ItemTags).Load();
            context.Entry(item).Collection(i => i.Comments).Load();
            context.Entry(item).Collection(i => i.Likes).Load();
            return item;
        }
        protected async override Task AddEntity(Item entity)
        {
            await context.Items.AddAsync(entity);
        }
        protected override void RemoveEntity(Item entity)
        {
            RemoveFieldValues(entity);
            RemoveComments(entity);
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
        private void RemoveComments(Item entity)
        {
            context.Comments.RemoveRange(context.Comments.Where(c => c.ItemId == entity.Id));
        }
        private void RemoveItemTags(Item entity)
        {
            context.ItemTags.RemoveRange(context.ItemTags.Where(it => it.ItemId == entity.Id));
        }
    }
}

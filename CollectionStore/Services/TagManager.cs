using CollectionStore.Data;
using CollectionStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Services
{
    public class TagManager : BaseEntityManager<Tag, int>
    {
        public TagManager(ApplicationDbContext context) : base(context) { }

        protected override Tag GetById(int id) => context.Tags.FirstOrDefault(t => t.Id == id);
        protected override Tag GetByIdCoherentlyLoad(int id)
        {
            return context.Tags.Where(t => t.Id == id)
                .Include(t => t.ItemTags)
                .ThenInclude(it => it.Item)
                .ThenInclude(i => i.FieldValues).FirstOrDefault(t => t.Id == id);
        }
        protected async override Task AddEntity(Tag entity)
        {
            if (entity != null && (await context.Tags.FirstOrDefaultAsync(t => t.Content == entity.Content)) == null)
            {
                await context.Tags.AddAsync(entity);
            }
        }
        protected override void RemoveEntity(Tag entity)
        {
            if(entity != null && context.Tags.FirstOrDefault(t => t.Id == entity.Id) != null)
            {
                context.Tags.Remove(entity);
            }
        }
        protected override void UpdateEntity(Tag entity, Tag sourceEntity)
        {
            if(entity != null && sourceEntity != null)
            {
                entity.Content = sourceEntity.Content;
                entity.ItemTags = sourceEntity.ItemTags;
            }
        }
    }
}

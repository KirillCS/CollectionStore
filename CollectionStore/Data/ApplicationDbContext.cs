using CollectionStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<CollectionTheme> CollectionThemes { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<FieldType> FieldTypes { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldValue> FieldValues { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ItemTag>()
                .HasKey(it => new { it.ItemId, it.TagId });

            builder.Entity<ItemTag>()
                .HasOne(it => it.Item)
                .WithMany(i => i.ItemTags)
                .HasForeignKey(it => it.ItemId);

            builder.Entity<ItemTag>()
                .HasOne(it => it.Tag)
                .WithMany(t => t.ItemTags)
                .HasForeignKey(it => it.TagId);

            base.OnModelCreating(builder);
        }
    }
}

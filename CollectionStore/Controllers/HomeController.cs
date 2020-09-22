using Microsoft.AspNetCore.Mvc;
using CollectionStore.Models;
using CollectionStore.ViewModels;
using CollectionStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using CollectionStore.Helpers;

namespace CollectionStore.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;

        public HomeController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index(SortBy sortBy = SortBy.None) => View(new HomeViewModel
        {
            Tags = GetTags(),
            Collections = GetCollections(),
            Items = GetItems(sortBy)
        });

        private List<Tag> GetTags()
        {
            return context.Tags.Include(t => t.ItemTags)
                .Where(t => t.ItemTags.Count > 0)
                .OrderByDescending(t => t.ItemTags.Count)
                .ToList();
        }
        private List<Collection> GetCollections()
        {
            return context.Collections
                .Include(c => c.Theme)
                .Include(c => c.User)
                .Include(c => c.Items)
                .ToList();
        }
        private List<Item> GetItems(SortBy sortBy)
        {
            var items = context.Items.Include(i => i.Collection)
                .ThenInclude(c => c.User)
                .Include(i => i.FieldValues)
                .ThenInclude(fv => fv.Field)
                .ThenInclude(f => f.Type)
                .Include(i => i.Likes)
                .OrderByDescending(i => i.Id)
                .ToList();
            return Sorting.SortItemsByParameter(items, sortBy);
        }
    }
}

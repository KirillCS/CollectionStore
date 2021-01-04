using System.Collections.Generic;
using System.Linq;
using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionStore.Controllers
{
    [AllowAnonymous]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext context;

        public SearchController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Search(string searchString = null)
        {
            searchString ??= string.Empty;
            return View(new SearchViewModel 
            {
                SearchParameter = searchString,
                Items = GetItemsByString(searchString),
                ByTag = false
            });
        }
        [HttpGet]
        public IActionResult SearchByTag(string tagContent = null)
        {
            tagContent ??= string.Empty;
            return View("Search", new SearchViewModel
            {
                SearchParameter = tagContent,
                Items = GetItemsByTag(tagContent),
                ByTag = true
            });
        }

        private List<Item> GetItemsByString(string searchString)
        {
            var items = context.Items.Where(i => EF.Functions.FreeText(i.Name, searchString) ||
                context.Collections.Where(c => c.Id == i.CollectionId && EF.Functions.FreeText(c.Name, searchString)).Count() != 0 ||
                context.Collections.Where(c => c.Id == i.CollectionId && EF.Functions.FreeText(c.Description, searchString)).Count() != 0 ||
                context.Comments.Where(c => c.ItemId == i.Id && EF.Functions.FreeText(c.Message, searchString)).Count() != 0)
                    .Include(i => i.Collection)
                    .ThenInclude(c => c.User)
                    .Include(i => i.Likes)
                    .OrderByDescending(i => i.Id)
                    .ToList();
            return items;
        }
        private List<Item> GetItemsByTag(string tagContent)
        {
            return context.ItemTags.Include(it => it.Tag)
                .Include(it => it.Item)
                .ThenInclude(i => i.Collection)
                .ThenInclude(c => c.User)
                .Include(it => it.Item)
                .ThenInclude(i => i.Likes)
                .Where(it => it.Tag.Content == tagContent)
                .Select(it => it.Item)
                .OrderByDescending(i => i.Id)
                .ToList();
        }
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CollectionStore.Models;
using Microsoft.AspNetCore.Http;
using CollectionStore.ViewModels;
using CollectionStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

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

        public IActionResult Index() => View(new HomeViewModel
        {
            Tags = GetTags(),
            Collections = GetCollections(),
            Items = GetItems()
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
            return context.Collections.Include(c => c.Theme)
                .Include(c => c.User)
                .ToList();
        }
        private List<Item> GetItems()
        {
            return context.Items.Include(i => i.Collection)
                .ThenInclude(c => c.User)
                .Include(i => i.FieldValues)
                .ThenInclude(fv => fv.Field)
                .ThenInclude(f => f.Type)
                .OrderByDescending(i => i.Id)
                .ToList();
        }
    }
}

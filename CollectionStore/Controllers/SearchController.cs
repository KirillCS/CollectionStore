using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                SearchString = searchString,
                Items = GetItems(searchString)
            });
        }

        private List<Item> GetItems(string searchString)
        {
            var items = new List<Item>();
            items = context.Items.Where(i => /*EF.Functions.FreeText(i.Name, searchString)*/i.Name.Contains(searchString))
                .Include(i => i.Collection)
                .ThenInclude(c => c.User)
                .OrderByDescending(i => i.Id)
                .ToList();
            return items;
        }
    }
}

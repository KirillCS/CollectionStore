﻿using System.Collections.Generic;
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
            var items = new List<Item>();
            items = context.Items.Where(i => /*EF.Functions.FreeText(i.Name, searchString)*/i.Name.Contains(searchString))
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

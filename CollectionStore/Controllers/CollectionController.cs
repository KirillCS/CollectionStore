﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CollectionStore.Controllers
{
    public class CollectionController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext context;
        private readonly IStringLocalizer<CollectionController> localizer;

        public CollectionController(UserManager<User> userManager, 
            ApplicationDbContext context, IStringLocalizer<CollectionController> localizer)
        {
            this.userManager = userManager;
            this.context = context;
            this.localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Add(string userName = null, string returnUrl = null)
        {
            userName ??= string.Empty;
            var user = await userManager.FindByNameAsync(userName);
            if(user == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["UserNotFoundTitle"],
                    ErrorMessage = localizer["UserNotFoundMessage"]
                });
            }
            return View(new AddingCollectionViewModel
            {
                Themes = context.CollectionThemes.ToList(),
                Types = context.FieldTypes.ToList(),
                UserId = user.Id,
                ReturnUrl = returnUrl
            });
        }
        [HttpPost]
        public IActionResult Add(AddingCollectionViewModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var collection = new Collection
                {
                    Name = model.Name,
                    Description = model.Description,
                    ThemeId = model.SelectedThemeId,
                    UserId = model.UserId,
                    Fields = GetFields(model)
                };
                context.Collections.Add(collection);
                context.SaveChanges();
                return Redirect(model.ReturnUrl);
            }
            model.Themes = context.CollectionThemes.ToList();
            model.Types = context.FieldTypes.ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Remove(int collectionId, string returnUrl = null)
        {
            returnUrl ??= "~/";
            var collection = context.Collections.SingleOrDefault(c => c.Id == collectionId);
            if(collection == null)
            {
                return View("Error", new ErrorViewModel 
                { 
                    ErrorTitle = localizer["CollectionNotFoundTitle"],
                    ErrorMessage = localizer["CollectionNotFoundMessage"],
                    ButtonLabel = localizer["CollectionNotFoundButtonLabel"],
                    Url = returnUrl
                });
            }
            await RemoveCollection(collection);
            return Redirect(returnUrl);
        }

        private List<Field> GetFields(AddingCollectionViewModel model)
        {
            var fields = new List<Field>();
            if (!model.FieldNames.IsNullOrEmpty() && !model.FieldTypesIds.IsNullOrEmpty())
            {
                for (int i = 0; i < model.FieldNames.Count; i++)
                {
                    fields.Add(new Field
                    {
                        Name = model.FieldNames[i],
                        TypeId = model.FieldTypesIds[i]
                    });
                }
            }
            return fields;
        }
        private async Task RemoveCollection(Collection collection)
        {
            RemoveItems(collection);
            RemoveFields(collection);
            context.Collections.Remove(collection);
            await context.SaveChangesAsync();
        }
        private void RemoveItems(Collection collection)
        {
            foreach (var item in collection.Items)
            {
                context.FieldValues.RemoveRange(context.FieldValues.Where(fv => fv.ItemId == item.Id));
            }
            context.Items.RemoveRange(context.Items.Where(i => i.CollectionId == collection.Id));
        }
        private void RemoveFields(Collection collection)
        {
            context.Fields.RemoveRange(context.Fields.Where(f => f.CollectionId == collection.Id));
        }
    }
}

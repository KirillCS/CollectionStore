using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.Services;
using CollectionStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CollectionStore.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly ItemManager itemManager;
        private readonly CollectionManager collectionManager;
        private readonly TagManager tagManager;
        private readonly UserChecker userChecker;
        private readonly IStringLocalizer<ItemController> localizer;

        public ItemController(ApplicationDbContext context, 
            ItemManager itemManager, CollectionManager collectionManager,
            TagManager tagManager, UserChecker userChecker, 
            IStringLocalizer<ItemController> localizer)
        {
            this.context = context;
            this.itemManager = itemManager;
            this.collectionManager = collectionManager;
            this.tagManager = tagManager;
            this.userChecker = userChecker;
            this.localizer = localizer;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(int itemId, string returnUrl = null)
        {
            var item = context.Items.Where(i => i.Id == itemId)
                .Include(i => i.Collection)
                .ThenInclude(c => c.User)
                .Include(i => i.FieldValues)
                .ThenInclude(fv => fv.Field)
                .ThenInclude(f => f.Type)
                .Include(i => i.ItemTags)
                .ThenInclude(it => it.Tag)
                .Include(i => i.Comments)
                .ThenInclude(c => c.User)
                .SingleOrDefault(i => i.Id == itemId);
            if(item == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["ItemNotFoundTitle"],
                    ErrorMessage = localizer["ItemNotFoundMessage"],
                    ButtonLabel = localizer["Back"],
                    Url = returnUrl
                });
            }
            return View(new ItemViewModel 
            {
                Item = item,
                ReturnUrl = returnUrl
            });
        }

        [HttpGet]
        public async Task<IActionResult> Add(int collectionId, string returnUrl = null)
        {
            var collection = collectionManager.GetById(collectionId, true);
            if(collection == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["CollectionNotFoundTitle"],
                    ErrorMessage = localizer["CollectionNotFoundMessage"]
                });
            }

            var view = await CheckUser(collection.User.UserName);
            if (view != null) return view;

            return View("AddEdit", new AddingEditingItemViewModel
            {
                CollectionId = collectionId,
                Collection = collection,
                ReturnUrl = returnUrl
            });
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddingEditingItemViewModel model)
        {
            model.ReturnUrl ??= "~/";
            model.Collection = collectionManager.GetById(model.CollectionId, true);
            if (model.Collection == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["CollectionNotFoundTitle"],
                    ErrorMessage = localizer["CollectionNotFoundMessage"]
                });
            }

            var view = await CheckUser(model.Collection.User.UserName);
            if (view != null) return view;

            if(!ValidateFields(model))
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["ValidationErrorMessage"],
                    ErrorMessage = localizer["ValidationErrorTitle", model.Collection.Name],
                    ButtonLabel = localizer["ToCollectionPage"],
                    Url = model.ReturnUrl
                });
            }
            if (ModelState.IsValid)
            {
                var item = new Item
                {
                    Name = model.Name,
                    CollectionId = model.CollectionId,
                    FieldValues = GetFieldValues(model)
                };
                if(await itemManager.AddAsync(item) == OperationResult.Failed)
                {
                    return View("Error", new ErrorViewModel
                    {
                        ErrorTitle = localizer["AddingItemErrorTitle"],
                        ErrorMessage = localizer["AddingItemErrorMessage"],
                        ButtonLabel = localizer["ToCollectionPage"],
                        Url = model.ReturnUrl
                    });
                }
                await AddTagsAsync(item.Id, model);
                return Redirect(model.ReturnUrl);
            }
            return View("AddEdit", model);
        }

        [HttpGet]
        public async Task<IActionResult> Remove(int itemId, string returnUrl = null)
        {
            returnUrl ??= "~/";
            var item = await context.Items.Where(i => i.Id == itemId)
                                          .Include(i => i.Collection)
                                          .ThenInclude(c => c.User)
                                          .SingleOrDefaultAsync(i => i.Id == itemId);
            if(item != null)
            {
                var view = await CheckUser(item.Collection.User.UserName);
                if (view != null) return view;
                if(await itemManager.RemoveAsync(item.Id) == OperationResult.Failed)
                {
                    return View("Error", new ErrorViewModel
                    {
                        ErrorTitle = localizer["RemovingItemErrorTitle"],
                        ErrorMessage = localizer["RemovingItemErrorMessage"],
                        ButtonLabel = localizer["ToCollectionPage"],
                        Url = returnUrl
                    });
                }
            }
            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int itemId, string returnUrl = null)
        {
            var item = await context.Items
                .Where(i => i.Id == itemId)
                .Include(i => i.FieldValues)
                .Include(i => i.Collection)
                .ThenInclude(c => c.User)
                .Include(i => i.Collection)
                .ThenInclude(c => c.Fields)
                .ThenInclude(f => f.Type)
                .Include(i => i.ItemTags)
                .ThenInclude(it => it.Tag)
                .SingleOrDefaultAsync(i => i.Id == itemId);
            if (item == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["ItemNotFoundTitle"],
                    ErrorMessage = localizer["ItemNotFoundMessage"],
                    ButtonLabel = localizer["ToCollectionPage"],
                    Url = returnUrl
                });
            }

            var view = await CheckUser(item.Collection.User.UserName);
            if (view != null) return view;

            return View("AddEdit", new AddingEditingItemViewModel 
            {
                ItemId = itemId,
                Name = item.Name,
                TagNames = item.ItemTags.Select(it => it.Tag.Content).ToList(),
                Values = item.FieldValues.Select(fv => fv.Value).ToList(),
                CollectionId = item.Collection.Id,
                Collection = item.Collection,
                ReturnUrl = returnUrl,
                IsEditing = true
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AddingEditingItemViewModel model)
        {
            model.ReturnUrl ??= "~/";
            model.Collection = collectionManager.GetById(model.CollectionId, true);
            if(model.Collection == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["CollectionNotFoundTitle"],
                    ErrorMessage = localizer["CollectionNotFoundMessage"]
                });
            }

            var view = await CheckUser(model.Collection.User.UserName);
            if (view != null) return view;

            var item = context.Items.Where(i => i.Id == model.ItemId).Include(i => i.FieldValues).SingleOrDefault(i => i.Id == model.ItemId);
            if(item == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["ItemNotFoundTitle"],
                    ErrorMessage = localizer["ItemNotFoundMessage"],
                    ButtonLabel = localizer["ToCollectionPage"],
                    Url = model.ReturnUrl
                });
            }
            if (!ValidateFields(model))
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["ValidationErrorMessage"],
                    ErrorMessage = localizer["ValidationErrorTitle", model.Collection.Name],
                    ButtonLabel = localizer["ToCollectionPage"],
                    Url = model.ReturnUrl
                });
            }
            if (ModelState.IsValid)
            {
                await EditItem(item, model);
                return Redirect(model.ReturnUrl);
            }
            return View("AddEdit", model);
        }

        private async Task<IActionResult> CheckUser(string ownerUserName)
        {
            var error = await userChecker.CheckUserExistence();
            if (error != null) return View("Error", error);
            error = await userChecker.CheckUserBlockStatus();
            if (error != null) return View("Error", error);
            error = userChecker.CheckUserAccess(ownerUserName);
            if (error != null) return View("Error", error);
            return null;
        }
        private bool ValidateFields(AddingEditingItemViewModel model)
        {
            foreach (int id in model.FieldIds)
            {
                if(!model.Collection.Fields.Any(f => f.Id == id))
                {
                    return false;
                }
            }
            return true;
        }
        private List<FieldValue> GetFieldValues(AddingEditingItemViewModel model)
        {
            var fieldValues = new List<FieldValue>();
            for (int i = 0; i < model.FieldIds.Count; i++)
            {
                fieldValues.Add(new FieldValue
                {
                    FieldId = model.FieldIds[i],
                    Value = model.Values[i]
                });
            }
            return fieldValues;
        }
        private async Task AddTagsAsync(int itemId, AddingEditingItemViewModel model)
        {
            foreach (string content in model.TagNames)
            {
                await itemManager.AddTagAsync(content, itemId, false);
            }
            await context.SaveChangesAsync();
        }
        private async Task EditItem(Item item, AddingEditingItemViewModel model)
        {
            item.Name = model.Name;
            for (int i = 0; i < model.FieldIds.Count; i++)
            {
                var fieldValue = item.FieldValues.SingleOrDefault(fv => fv.FieldId == model.FieldIds[i]);
                if(fieldValue != null)
                {
                    fieldValue.Value = model.Values[i];
                }
            }
            context.ItemTags.RemoveRange(context.ItemTags.Where(it => it.ItemId == item.Id));
            await AddTagsAsync(item.Id, model);
            await context.SaveChangesAsync();
        }
    }
}

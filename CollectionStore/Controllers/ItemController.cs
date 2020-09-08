using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionStore.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext context;

        public ItemController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Add(int? collectionId = null, string returnUrl = null)
        {
            collectionId ??= -1;
            var collection = GetCollection(collectionId.Value);
            if(collection == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = "Collection isn't found!",
                    ErrorMessage = "Collection is not found!"
                });
            }
            return View(new AddingItemViewModel
            {
                CollectionId = collectionId.Value,
                Collection = collection,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddingItemViewModel model)
        {
            model.Collection = GetCollection(model.CollectionId);
            if (model.Collection == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = "Collection isn't found!",
                    ErrorMessage = "Collection is not found!"
                });
            }
            if(!ValidateFields(model))
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = "Something went wrong :(",
                    ErrorMessage = "Someone remove one or more fields."
                });
            }
            if (ModelState.IsValid)
            {
                await AddItem(model);
                return Redirect(model.ReturnUrl);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Remove(int itemId, string returnUrl = null)
        {
            returnUrl ??= "~/";
            var item = await context.Items.SingleOrDefaultAsync(i => i.Id == itemId);
            if(item != null)
            {
                await RemoveItem(item);
            }
            return Redirect(returnUrl);
        }

        private Collection GetCollection(int id) => context.Collections
                .Where(c => c.Id == id)
                .Include(c => c.Fields)
                .ThenInclude(f => f.Type)
                .SingleOrDefault(c => c.Id == id);
        private bool ValidateFields(AddingItemViewModel model)
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
        private async Task AddItem(AddingItemViewModel model)
        {
            context.Items.Add(new Item
            {
                Name = model.Name,
                CollectionId = model.CollectionId,
                FieldValues = GetFieldValues(model)
            });
            await context.SaveChangesAsync();
        }
        private List<FieldValue> GetFieldValues(AddingItemViewModel model)
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
        private async Task RemoveItem(Item item)
        {
            context.FieldValues.RemoveRange(context.FieldValues.Where(fv => fv.ItemId == item.Id));
            context.Items.Remove(item);
            await context.SaveChangesAsync();
        }
    }
}

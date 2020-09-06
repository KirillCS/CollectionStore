using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
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
            var collection = context.Collections.Where(c => c.Id == collectionId).Include(c => c.Fields).ThenInclude(f => f.Type).SingleOrDefault(c => c.Id == collectionId);
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
                CollectionId = collectionId,
                CollectionName = collection.Name,
                Fields = collection.Fields,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public IActionResult Add(AddingItemViewModel model)
        {

            return null;
        }
    }
}

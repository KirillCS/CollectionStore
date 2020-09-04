using System;
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

namespace CollectionStore.Controllers
{
    public class CollectionController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CollectionController(UserManager<User> userManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
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
                    ErrorTitle = "User isn't found",
                    ErrorMessage = "User isn't found"
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
    }
}

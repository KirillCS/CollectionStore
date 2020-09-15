using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.Services;
using CollectionStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CollectionStore.Controllers
{
    [Authorize]
    public class CollectionController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext context;
        private readonly CollectionService collectionService;
        private readonly IStringLocalizer<CollectionController> localizer;

        public CollectionController(UserManager<User> userManager, 
            ApplicationDbContext context, CollectionService collectionService, 
            IStringLocalizer<CollectionController> localizer)
        {
            this.userManager = userManager;
            this.context = context;
            this.collectionService = collectionService;
            this.localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Add(string userName = null, string returnUrl = null)
        {
            userName ??= string.Empty;
            if (User.Identity.Name != userName && !User.IsInRole(Role.Admin))
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["NotRightsTitle"],
                    ErrorMessage = localizer["NotRightsMessage", userName]
                });
            }
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
        public async Task<IActionResult> Add(AddingCollectionViewModel model)
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
                if(await collectionService.AddAsync(collection) == OperationResult.Failed)
                {
                    return View("Error", new ErrorViewModel
                    {
                        ErrorTitle = localizer["AddingCollectionErrorTitle"],
                        ErrorMessage = localizer["AddingCollectionErrorMessage"],
                        ButtonLabel = localizer["ToProfile"],
                        Url = model.ReturnUrl
                    });
                }
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
            var collection = context.Collections.Where(c => c.Id == collectionId).Include(c => c.User).SingleOrDefault(c => c.Id == collectionId);
            if(collection != null)
            {
                var userName = collection.User.UserName;
                if (User.Identity.Name != userName || !User.IsInRole(Role.Admin))
                {
                    return View("Error", new ErrorViewModel
                    {
                        ErrorTitle = localizer["NotRightsTitle"],
                        ErrorMessage = localizer["NotRightsMessage", userName]
                    });
                }
                if(await collectionService.RemoveAsync(collection) == OperationResult.Failed)
                {
                    return View("Error", new ErrorViewModel
                    {
                        ErrorTitle = localizer["RemovingCollectionErrorTitle"],
                        ErrorMessage = localizer["RemovingCollectionErrorMessage"],
                        ButtonLabel = localizer["Back"],
                        Url = returnUrl
                    });
                }
            }
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
    }
}

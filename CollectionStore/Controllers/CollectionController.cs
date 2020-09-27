using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.Services;
using CollectionStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly UserChecker userChecker;
        private readonly CollectionManager collectionService;
        private readonly IStringLocalizer<CollectionController> localizer;
        private readonly IBlobService blobService;

        public CollectionController(UserManager<User> userManager, 
            ApplicationDbContext context, UserChecker userChecker,
            CollectionManager collectionService, IStringLocalizer<CollectionController> localizer,
            IBlobService blobService)
        {
            this.userManager = userManager;
            this.context = context;
            this.userChecker = userChecker;
            this.collectionService = collectionService;
            this.localizer = localizer;
            this.blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> Add(string userName = null, string returnUrl = null)
        {
            var view = await CheckUser(userName);
            if (view != null) return view;

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
                UserName = user.UserName,
                ReturnUrl = returnUrl
            });
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddingCollectionViewModel model)
        {
            var view = await CheckUser(model.UserName);
            if (view != null) return view;

            model.ReturnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var collection = await CreateCollection(model);
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
                var view = await CheckUser(collection.User.UserName);
                if (view != null) return view;

                await DeleteImage(collection);
                if (await collectionService.RemoveAsync(collection.Id) == OperationResult.Failed)
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
        [HttpPost]
        public async Task<IActionResult> EditInfo(EditingCollectionInfoViewModel model)
        {
            var collection = collectionService.GetById(model.CollectionId, true);
            if(collection != null)
            {
                var error = await CheckUser(collection.User.UserName);
                if (error != null) return error;
                collection.Name = model.Name;
                collection.ThemeId = model.ThemeId;
                collection.Description = model.Description;
                await context.SaveChangesAsync();
            }
            return RedirectToAction("Collection", "Profile", new { collectionId = model.CollectionId, returnUrl = model.ReturnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> EditCover(EditingCollectionCoverViewModel model)
        {
            var collection = collectionService.GetById(model.CollectionId, true);
            if (collection != null)
            {
                var error = await CheckUser(collection.User.UserName);
                if (error != null) return error;
                await DeleteImage(collection);
                collection.ImagePath = await UploadImage(model.File);
                await context.SaveChangesAsync();
            }
            return RedirectToAction("Collection", "Profile", new { collectionId = model.CollectionId, returnUrl = model.ReturnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> EditFields(EditingCollectionFieldsViewModel model)
        {
            var collection = collectionService.GetById(model.CollectionId, true);
            if (collection != null)
            {
                var error = await CheckUser(collection.User.UserName);
                if (error != null) return error;
                await JoinFieldsInCollection(collection, GetFields(model.FieldNames, model.FieldTypesIds));
            }
            return RedirectToAction("Collection", "Profile", new { collectionId = model.CollectionId, returnUrl = model.ReturnUrl });
        }

        private async Task<Collection> CreateCollection (AddingCollectionViewModel model)
        {
            return new Collection
            {
                Name = model.Name,
                Description = model.Description,
                ImagePath = await UploadImage(model.File),
                ThemeId = model.SelectedThemeId,
                UserId = model.UserId,
                Fields = GetFields(model.FieldNames, model.FieldTypesIds)
            };
        }
        private async Task<string> UploadImage(IFormFile file)
        {
            string blobUri = null;
            if (file != null)
            {
                blobUri = await blobService.UploadFileBlobAsync(file.OpenReadStream(), file.FileName);
            }
            return blobUri;
        }
        private List<Field> GetFields(List<string> fieldNames, List<int> fieldTypesIds)
        {
            var fields = new List<Field>();
            if (!fieldNames.IsNullOrEmpty() && !fieldTypesIds.IsNullOrEmpty() && fieldNames.Count == fieldTypesIds.Count)
            {
                for (int i = 0; i < fieldNames.Count; i++)
                {
                    fields.Add(new Field
                    {
                        Name = fieldNames[i],
                        TypeId = fieldTypesIds[i]
                    });
                }
            }
            return fields;
        }
        private async Task JoinFieldsInCollection(Collection collection, List<Field> modelFields)
        {
            await AddFieldsToCollection(collection, modelFields);
            collection = collectionService.GetById(collection.Id, true);
            AddFieldValuesToItems(collection);
            RemoveFieldsFromCollection(collection, modelFields);
            await context.SaveChangesAsync();
        }
        private async Task AddFieldsToCollection(Collection collection, List<Field> modelFields)
        {
            foreach (var modelField in modelFields)
            {
                if (!collection.Fields.Any(f => f.Name == modelField.Name && f.TypeId == modelField.TypeId))
                {
                    collection.Fields.Add(new Field { Name = modelField.Name, TypeId = modelField.TypeId });
                }
            }
            await context.SaveChangesAsync();
        }
        private void AddFieldValuesToItems(Collection collection)
        {
            foreach (var field in collection.Fields)
            {
                foreach (var item in collection.Items)
                {
                    if (!item.FieldValues.Any(fv => fv.FieldId == field.Id))
                    {
                        item.FieldValues.Add(new FieldValue { FieldId = field.Id, Value = field.Type.Name == "Check box" ? "unchecked" : null });
                    }
                }
            }
        }
        private void RemoveFieldsFromCollection(Collection collection, List<Field> modelFields)
        {
            for (int i = 0; i < collection.Fields.Count; i++)
            {
                var field = collection.Fields[i];
                if (!modelFields.Any(mf => mf.Name == field.Name && mf.TypeId == field.TypeId))
                {
                    context.FieldValues.RemoveRange(context.FieldValues.Where(fv => fv.FieldId == field.Id));
                    collection.Fields.Remove(field);
                    i--;
                }
            }
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
        private async Task DeleteImage(Collection collection)
        {
            if (!string.IsNullOrEmpty(collection.ImagePath))
            {
                await blobService.DeleteBlobAsync(Path.GetFileName(collection.ImagePath));
            }
        }
    }
}

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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollectionStore.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext context;
        private readonly UserChecker userChecker;
        private readonly CollectionManager collectionService;
        private readonly IBlobService blobService;

        public AdminController(UserManager<User> userManager,
            ApplicationDbContext context, UserChecker userChecker, 
            CollectionManager collectionService, IBlobService blobService)
        {
            this.userManager = userManager;
            this.context = context;
            this.userChecker = userChecker;
            this.collectionService = collectionService;
            this.blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> AdminPanel() => View(await GetViewModel());
        [HttpPost]
        public async Task<IActionResult> Manage(string actionName, List<string> selectedIds)
        {
            var view = await CheckUser();
            if (view != null) return view;
            await DoAction(SetAction(actionName), selectedIds);
            return RedirectToAction("AdminPanel");
        }

        private async Task<List<AdminPanelViewModel>> GetViewModel()
        {
            var model = new List<AdminPanelViewModel>();
            var users = userManager.Users.ToList();
            for (int i = 0; i < users.Count; i++)
            {
                if(User.Identity.Name != users[i].UserName)
                {
                    model.Add(new AdminPanelViewModel
                    {
                        Id = users[i].Id,
                        UserName = users[i].UserName,
                        IsBlocked = users[i].IsBlocked,
                        IsAdmin = await userManager.IsInRoleAsync(users[i], Role.Admin)
                    });
                }
            }
            return model;
        }
        private async Task<IActionResult> CheckUser()
        {
            var error = await userChecker.CheckUserExistence();
            if (error != null) return View("Error", error);
            error = await userChecker.CheckUserBlockStatus();
            if (error != null) return View("Error", error);
            return null;
        }
        private Func<User, Task<IdentityResult>> SetAction(string actionName)
        {
            if (actionName == "block" || actionName == "unblock")
            {
                return async user =>
                {
                    user.IsBlocked = actionName == "block";
                    return await userManager.UpdateAsync(user);
                };
            }
            else if(actionName == "delete")
            {
                return async user => await DeleteUser(user);
            }
            else if(actionName == "toUser" || actionName == "toAdmin")
            {
                return async user =>
                {
                    string oldRole = actionName == "toUser" ? Role.Admin : Role.User;
                    string newRole = actionName == "toAdmin" ? Role.Admin : Role.User;
                    if(!(await userManager.IsInRoleAsync(user, newRole)))
                    {
                        await userManager.RemoveFromRoleAsync(user, oldRole);
                        return await userManager.AddToRoleAsync(user, newRole);
                    }
                    return IdentityResult.Success;
                };
            }
            return async user => await Task.Run(() => IdentityResult.Failed(new IdentityError { Description = $"No action \"{actionName}\"" }));
        }
        private async Task DoAction(Func<User, Task<IdentityResult>> action, List<string> selectedIds)
        {
            User user = null;
            foreach (string id in selectedIds)
            {
                user = await userManager.FindByIdAsync(id);
                if(user != null)
                {
                    await action(user);
                }
            }
        }
        private async Task<IdentityResult> DeleteUser(User user)
        {
            await RemoveCollections(user);
            RemoveComments(user);
            RemoveLikes(user);
            return await userManager.DeleteAsync(user);
        }
        private async Task RemoveCollections(User user)
        {
            var collectionsIds = context.Collections.Where(c => c.UserId == user.Id).Select(c => c.Id).ToList();
            foreach (var id in collectionsIds)
            {
                await DeleteCover(id);
                await collectionService.RemoveAsync(id);
            }
        }
        private async Task DeleteCover(int collectionId)
        {
            var collection = collectionService.GetById(collectionId);
            if(collection != null && !string.IsNullOrEmpty(collection.ImagePath))
            {
                await blobService.DeleteBlobAsync(Path.GetFileName(collection.ImagePath));
            }
        }
        private void RemoveComments(User user)
        {
            context.Comments.RemoveRange(context.Comments.Where(c => c.UserId == user.Id));
        }
        private void RemoveLikes(User user)
        {
            context.Likes.RemoveRange(context.Likes.Where(l => l.UserId == user.Id));
        }
    }
}

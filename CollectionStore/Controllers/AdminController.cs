using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionStore.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> AdminPanel() => View(await GetViewModel());

        [HttpPost]
        public async Task<IActionResult> Manage(string actionName, List<string> selectedIds)
        {
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
                return async user => await userManager.DeleteAsync(user);
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

    }
}

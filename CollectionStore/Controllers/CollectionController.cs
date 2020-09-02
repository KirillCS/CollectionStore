using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollectionStore.Controllers
{
    public class CollectionController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext context;

        public CollectionController(UserManager<User> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

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
                ReturnUrl = returnUrl
            });
        }
    }
}

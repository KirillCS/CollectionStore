using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.ViewModels;
using CollectionStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using CollectionStore.Data;
using Microsoft.EntityFrameworkCore;
using CollectionStore.Helpers;

namespace CollectionStore.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext context;
        private readonly IStringLocalizer<ProfileController> localizer;

        public ProfileController(UserManager<User> userManager, 
            ApplicationDbContext context, IStringLocalizer<ProfileController> localizer)
        {
            this.userManager = userManager;
            this.context = context;
            this.localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string userName = null)
        {
            userName ??= string.Empty;
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["UserNotFoundTitle"],
                    ErrorMessage = localizer["UserNotFountMessage", string.IsNullOrEmpty(userName) ? "" : $" {userName}", "someaddress@problem.com"]
                });
            }
            return View(new ProfileViewModel 
            { 
                UserName = user.UserName,
                Collections = context.Collections.Include(c => c.Theme).Where(c => c.UserId == user.Id).ToList()
            });
        }
        [HttpGet]
        public IActionResult Collection(int collectionId, SortBy sortBy = SortBy.None, string returnUrl = null)
        {
            returnUrl ??= "~/";
            var collection = context.Collections
                .Where(c => c.Id == collectionId)
                .Include(c => c.Items)
                .ThenInclude(i => i.Likes)
                .Include(c => c.Theme)
                .Include(c => c.User)
                .SingleOrDefault(c => c.Id == collectionId);
            if(collection == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["CollectionNotFoundTitle"],
                    ErrorMessage = localizer["CollectionNotFountMessage", "someaddress@problem.com"]
                });
            }
            collection.Items = Sorting.SortItemsByParameter(collection.Items, sortBy);
            return View(new CollectionViewModel
            {
                Collection = collection,
                ReturnUrl = returnUrl
            });
        }
    }
}

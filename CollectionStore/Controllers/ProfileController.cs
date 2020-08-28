using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.ViewModels;
using CollectionStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CollectionStore.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IStringLocalizer<ProfileController> localizer;

        public ProfileController(UserManager<User> userManager, IStringLocalizer<ProfileController> localizer)
        {
            this.userManager = userManager;
            this.localizer = localizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id = null)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("Error", new ErrorViewModel
                {
                    ErrorTitle = localizer["UserNotFoundTitle", "someaddress@problem.com"],
                    ErrorMessage = localizer["UserNotFountMessage", "someaddress@problem.com"]
                });
            }
            return View();
        }
    }
}

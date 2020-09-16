using System.Threading.Tasks;
using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CollectionStore.Services
{
    public class UserChecker
    {
        private readonly HttpContext context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IStringLocalizer<UserChecker> localizer;

        public UserChecker(IHttpContextAccessor context, 
            UserManager<User> userManager, SignInManager<User> signInManager, 
            IStringLocalizer<UserChecker> localizer)
        {
            this.context = context.HttpContext;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.localizer = localizer;
        }

        public async Task<ErrorViewModel> CheckUserExistence()
        {
            string userName = context.User.Identity == null ? string.Empty : context.User.Identity.Name;
            var user = await userManager.FindByNameAsync(userName);
            return user != null ? null : 
                new ErrorViewModel 
                { 
                    ErrorTitle = localizer["UserNotFoundTitle"],
                    ErrorMessage = localizer["UserNotFoundMessage"] 
                };
        }
        public async Task<ErrorViewModel> CheckUserBlockStatus()
        {
            string userName = context.User.Identity == null ? string.Empty : context.User.Identity.Name;
            var user = await userManager.FindByNameAsync(userName);

            if (user != null && user.IsBlocked)
            {
                await signInManager.SignOutAsync();
                return new ErrorViewModel
                {
                    ErrorTitle = localizer["UserWasBlockedTitle"],
                    ErrorMessage = localizer["UserWasBlockedMessage"]
                };
            }
            return null;
        }
        public async Task<bool> IsUserBlocked(string userName)
        {
            var user = await userManager.FindByNameAsync(userName ?? string.Empty);
            return user != null && user.IsBlocked;
        }
        public ErrorViewModel CheckUserAccess(string ownerUserName)
        {
            string currentUserName = context.User.Identity == null ? string.Empty : context.User.Identity.Name;
            bool isAdmin = context.User.Identity != null && context.User.IsInRole(Role.Admin);
            if (currentUserName != ownerUserName && !isAdmin)
            {
                return new ErrorViewModel
                {
                    ErrorTitle = localizer["NotRightsTitle"],
                    ErrorMessage = localizer["NotRightsMessage", currentUserName]
                };
            }
            return null;
        }
    }
}

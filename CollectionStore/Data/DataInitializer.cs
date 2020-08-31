using CollectionStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CollectionStore.Data
{
    public class DataInitializer
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private const string AdminRoleName = "admin";
        private const string UserRoleName = "user";
        private string adminName;
        private string adminPassword;

        public DataInitializer(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;

            adminName = configuration["Admin:Login"] ?? "Admin";
            adminPassword = configuration["Admin:Password"] ?? "Qwe123!";
        }

        public async Task InitializeRoleAsync()
        {
            await CreateRoleIfNull(AdminRoleName, UserRoleName);
        }
        public async Task InitializeAdminAsync()
        {
            if((await userManager.FindByNameAsync(AdminRoleName)) == null)
            {
                var admin = new User { UserName = adminName };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, AdminRoleName);
                }
            }
        }
        private async Task CreateRoleIfNull (params string[] roles)
        {
            foreach (string role in roles)
            {
                if (!(await roleManager.RoleExistsAsync(role)))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}

using CollectionStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Data
{
    public class DataInitializer
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly string[] CollectionThemes =
        {
            "Others",
            "Phones",
            "Wines",
            "Coins",
            "Stamps"
        };
        private readonly string[] FieldTypes =
        {
            "Numeric",
            "String",
            "Markdown",
            "Date",
            "Check box"
        };
        private string adminName;
        private string adminPassword;

        public DataInitializer(UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, IConfiguration configuration, 
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.context = context;
            adminName = configuration["Admin:Login"] ?? "Admin";
            adminPassword = configuration["Admin:Password"] ?? "Qwe123!";
        }

        public async Task InitializeRoleAsync()
        {
            await AddRoleIfNull(Role.Admin, Role.User);
        }
        public async Task InitializeAdminAsync()
        {
            if((await userManager.FindByNameAsync(Role.Admin)) == null)
            {
                var admin = new User { UserName = adminName };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Role.Admin);
                }
            }
        }
        public async Task InitializeCollectionThemes()
        {
            var collectionThemes = from theme in CollectionThemes
                                   where !context.CollectionThemes.Any(t => t.Name == theme)
                                   select new CollectionTheme { Name = theme };
            context.CollectionThemes.AddRange(collectionThemes);
            await context.SaveChangesAsync();
        }
        public async Task InitializeFieldTypes()
        {
            var fieldTypes = from type in FieldTypes
                             where !context.FieldTypes.Any(t => t.Name == type)
                             select new FieldType { Name = type };
            context.FieldTypes.AddRange(fieldTypes);
            await context.SaveChangesAsync();
        }
        private async Task AddRoleIfNull (params string[] roles)
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

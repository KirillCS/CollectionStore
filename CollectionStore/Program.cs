using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
using CollectionStore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CollectionStore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await DbInitialize(host);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static async Task DbInitialize(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var initializer = new DataInitializer(services.GetRequiredService<UserManager<User>>(),
                        services.GetRequiredService<RoleManager<IdentityRole>>(), services.GetRequiredService<IConfiguration>(),
                        services.GetRequiredService<ApplicationDbContext>());
                    await initializer.InitializeRoleAsync();
                    await initializer.InitializeAdminAsync();
                    await initializer.InitializeCollectionThemes();
                    await initializer.InitializeFieldTypes();
                }
                catch (Exception ex)
                {
                    services.GetRequiredService<ILogger<Program>>().LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }
    }
}

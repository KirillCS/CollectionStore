using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CollectionStore.Models;
using Microsoft.AspNetCore.Http;
using CollectionStore.ViewModels;
using CollectionStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CollectionStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;

        public HomeController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index() => View(new HomeViewModel
        {
            Collections = context.Collections
                                 .Include(c => c.Theme)
                                 .Include(c => c.User)
                                 .ToList(),
            Items = context.Items
                           .Include(i => i.Collection)
                           .ThenInclude(c => c.User)
                           .Include(i => i.FieldValues)
                           .ThenInclude(fv => fv.Field)
                           .ThenInclude(f => f.Type)
                           .OrderByDescending(i => i.Id)
                           .ToList()
        });
    }
}

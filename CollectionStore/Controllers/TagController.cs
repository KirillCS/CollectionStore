using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace CollectionStore.Controllers
{
    public class TagController : Controller
    {
        private readonly ApplicationDbContext context;

        public TagController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetTags(string term = "")
        {
            try
            {
                var tags = context.Tags.Where(t => t.Content.Contains(term)).Select(t => new { value = t.Content }).ToList();
                return Json(tags);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}

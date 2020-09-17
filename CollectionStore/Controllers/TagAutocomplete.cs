using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionStore.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollectionStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagAutocomplete : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TagAutocomplete(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetTags(string term = "")
        {
            try
            {
                var tags = context.Tags.Where(t => t.Content.Contains(term)).Select(t => t.Content).ToList();
                return Ok(tags);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}

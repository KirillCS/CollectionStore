using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollectionStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<User> userManager;

        public LikeController(ApplicationDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<int>> Like(int itemId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!await CheckUserAndItem(itemId, userId))
            {
                return BadRequest();
            }
            if (!context.Likes.Any(l => l.ItemId == itemId && l.UserId == userId))
            {
                await LikeItem(itemId, userId);
            }
            else DislikeItem(itemId, userId);
            await context.SaveChangesAsync();
            return Ok(context.Likes.Where(l => l.ItemId == itemId).ToList().Count);
        }

        private async Task<bool> CheckUserAndItem(int itemId, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var item = context.Items.FirstOrDefault(i => i.Id == itemId);
            return user != null && !user.IsBlocked && item != null;
        }
        private async Task LikeItem(int itemId, string userId)
        {
            await context.Likes.AddAsync(new Like { ItemId = itemId, UserId = userId });
        }
        private void DislikeItem(int itemId, string userId)
        {
            context.Likes.Remove(context.Likes.FirstOrDefault(l => l.ItemId == itemId && l.UserId == userId));
        }
    }
}

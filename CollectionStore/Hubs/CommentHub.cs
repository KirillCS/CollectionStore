using CollectionStore.Data;
using CollectionStore.Models;
using CollectionStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CollectionStore.Hubs
{
    public class CommentHub : Hub
    {
        private readonly ApplicationDbContext context;
        private readonly ItemManager itemManager;

        public CommentHub(ApplicationDbContext context, ItemManager itemManager)
        {
            this.context = context;
            this.itemManager = itemManager;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ConnectionNotify");
            await base.OnConnectedAsync();
        }
        public async Task AddConnectionToGroup(string itemId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, itemId);
        }
        [Authorize]
        public async Task SendComment(string message, string itemId)
        {
            var item = itemManager.GetById(int.Parse(itemId));
            if(item != null)
            {
                await AddComment(message, item.Id);
            }
            await Clients.Group(itemId).SendAsync("SendComment", Context.User.Identity.Name, message, DateTime.Now.ToString("HH:mm dd.MM.yyyy"));
        }
        private async Task AddComment(string message, int itemId)
        {
            await context.Comments.AddAsync(new Comment 
            { 
                Message = message, 
                CreateDate = DateTime.Now,
                ItemId = itemId, 
                UserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            });
            await context.SaveChangesAsync();
        }
    }
}

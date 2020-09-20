using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }
    }
}

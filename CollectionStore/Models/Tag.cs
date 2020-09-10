using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public List<ItemTag> ItemTags { get; set; }

        public Tag()
        {
            ItemTags = new List<ItemTag>();
        }
    }
}

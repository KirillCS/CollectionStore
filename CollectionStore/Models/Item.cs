using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? CollectionId { get; set; }
        public Collection Collection { get; set; }

        public List<ItemField> Fields { get; set; }
        public List<ItemTag> ItemTags { get; set; }

        public Item()
        {
            Fields = new List<ItemField>();
            ItemTags = new List<ItemTag>();
        }
    }
}

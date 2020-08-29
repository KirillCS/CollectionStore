using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class ItemTag
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}

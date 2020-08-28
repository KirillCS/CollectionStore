using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class ItemField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public int? TypeId { get; set; }
        public ItemFieldType Type { get; set; }

        public int? ItemId { get; set; }
        public Item Item { get; set; }
    }
}

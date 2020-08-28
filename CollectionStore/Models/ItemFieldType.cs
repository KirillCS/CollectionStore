using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class ItemFieldType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ItemField> ItemFields { get; set; }

        public ItemFieldType()
        {
            ItemFields = new List<ItemField>();
        }
    }
}

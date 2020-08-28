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

        public List<Item> Items { get; set; }

        public Tag()
        {
            Items = new List<Item>();
        }
    }
}

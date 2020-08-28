using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        public int? ThemeId { get; set; }
        public CollectionTheme Theme { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public List<Item> Items { get; set; }

        public Collection()
        {
            Items = new List<Item>();
        }
    }
}

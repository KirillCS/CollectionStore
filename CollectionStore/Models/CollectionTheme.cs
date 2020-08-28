using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.Models
{
    public class CollectionTheme
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Collection> Collections { get; set; }

        public CollectionTheme()
        {
            Collections = new List<Collection>();
        }
    }
}

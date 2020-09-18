using CollectionStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class SearchViewModel
    {
        public string SearchParameter { get; set; }

        public List<Item> Items { get; set; }

        public bool ByTag { get; set; }

        public SearchViewModel()
        {
            Items = new List<Item>();
        }
    }
}

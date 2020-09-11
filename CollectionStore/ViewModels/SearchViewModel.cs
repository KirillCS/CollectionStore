using CollectionStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class SearchViewModel
    {
        public string SearchString { get; set; }

        public List<Item> Items { get; set; }

        public string ReturnUrl { get; set; }

        public SearchViewModel()
        {
            Items = new List<Item>();
        }
    }
}

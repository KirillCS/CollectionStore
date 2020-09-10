using CollectionStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class HomeViewModel
    {
        public List<Collection> Collections { get; set; }

        public List<Item> Items { get; set; }
    }
}

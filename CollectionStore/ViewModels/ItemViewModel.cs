using CollectionStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class ItemViewModel
    {
        public Item Item { get; set; }

        public string ReturnUrl { get; set; }
    }
}
